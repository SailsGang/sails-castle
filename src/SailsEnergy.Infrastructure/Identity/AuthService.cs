using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Auth.Commands.Login;
using SailsEnergy.Application.Features.Auth.Commands.RefreshToken;
using SailsEnergy.Application.Features.Auth.Commands.Register;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.ValueObjects;
using SailsEnergy.Infrastructure.Services;

namespace SailsEnergy.Infrastructure.Identity;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    IOptions<JwtSettings> settings,
    IAppDbContext dbContext,
    IAuditService auditService) : IAuthService
{
    private readonly JwtSettings _settings = settings.Value;

    public async Task<AuthResult> RegisterAsync(
        RegisterCommand command,
        CancellationToken ct = default)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser is not null)
            return AuthResult.Failure(ErrorCodes.EmailExists, "Email already registered.");

        if (command.Password != command.ConfirmPassword)
            return AuthResult.Failure(ErrorCodes.ValidationFailed, "Passwords do not match.");

        var user = new ApplicationUser
        {
            UserName = command.DisplayName,
            Email = command.Email
        };
        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return AuthResult.Failure(ErrorCodes.InvalidPassword, errors);
        }

        await userManager.AddToRoleAsync(user, nameof(UserRole.User));

        var profile = UserProfile.Create(user.Id, command.DisplayName, user.Id);
        if (command.FirstName is not null || command.LastName is not null)
            profile.SetName(command.FirstName, command.LastName, user.Id);
        dbContext.UserProfiles.Add(profile);

        user.UserProfileId = profile.Id;
        await userManager.UpdateAsync(user);

        var accessToken = jwtService.GenerateAccessToken(user.Id, command.Email, [nameof(UserRole.User)]);
        var refreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

        await userManager.UpdateAsync(user);

        await dbContext.SaveChangesAsync(ct);

        MetricsService.Registrations.Add(1);
        auditService.Log(new AuditEvent("REGISTER", "AUTH", user.Id, command.Email, user.Id, "User", true));

        return AuthResult.Ok(accessToken, refreshToken,
            DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            user.Id, command.Email, command.DisplayName);
    }

    public async Task<AuthResult> LoginAsync(
        LoginCommand command,
        CancellationToken ct = default)
    {
        MetricsService.LoginAttempts.Add(1);

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            MetricsService.LoginFailures.Add(1);
            auditService.Log(new AuditEvent("LOGIN_FAILED", "AUTH", null, command.Email, null, null, false, "Invalid credentials"));
            return AuthResult.Failure(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        var validPassword = await userManager.CheckPasswordAsync(user, command.Password);
        if (!validPassword)
        {
            MetricsService.LoginFailures.Add(1);
            auditService.Log(new AuditEvent("LOGIN_FAILED", "AUTH", null, command.Email, null, null, false, "Invalid credentials"));
            return AuthResult.Failure(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        var profile = user.UserProfileId.HasValue
            ? await dbContext.UserProfiles.FirstOrDefaultAsync(p => p.Id == user.UserProfileId.Value, ct)
            : null;

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtService.GenerateAccessToken(user.Id, command.Email, roles);
        var refreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

        await userManager.UpdateAsync(user);

        MetricsService.LoginSuccesses.Add(1);
        auditService.Log(new AuditEvent("LOGIN", "AUTH", user.Id, command.Email, null, null, true));

        return AuthResult.Ok(accessToken, refreshToken,
            DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            user.Id, command.Email, profile?.DisplayName ?? command.Email);
    }

    public async Task<AuthResult> RefreshTokenAsync(
        RefreshTokenCommand command,
        CancellationToken ct = default)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == command.RefreshToken, ct);
        if (user is null || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
            return AuthResult.Failure(ErrorCodes.InvalidRefreshToken, "Invalid or expired refresh token.");

        var profile = user.UserProfileId.HasValue
            ? await dbContext.UserProfiles.FirstOrDefaultAsync(p => p.Id == user.UserProfileId.Value, ct)
            : null;

        var roles = await userManager.GetRolesAsync(user);
        var newAccessToken = jwtService.GenerateAccessToken(user.Id, user.Email!, roles);
        var newRefreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
        await userManager.UpdateAsync(user);

        return AuthResult.Ok(newAccessToken, newRefreshToken,
            DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            user.Id, user.Email!, profile?.DisplayName ?? user.Email!);
    }

    public async Task LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await userManager.UpdateAsync(user);

        auditService.Log(new AuditEvent("LOGOUT", "AUTH", userId, null, null, null, true));
    }

}
