using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Auth.Commands;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Infrastructure.Identity;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    IOptions<JwtSettings> settings,
    IDocumentSession session) : IAuthService
{
    private readonly JwtSettings _settings = settings.Value;

    public async Task<AuthResult> RegisterAsync(
        string email,
        string password,
        string confirmPassword,
        string username,
        string? firstName,
        string? lastName,
        CancellationToken ct = default)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return AuthResult.Failure(ErrorCodes.EmailExists, "Email already registered.");

        if (password != confirmPassword)
            return AuthResult.Failure(ErrorCodes.ValidationFailed, "Passwords do not match.");

        var user = new ApplicationUser
        {
            UserName = username,
            Email = email,
        };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return AuthResult.Failure(ErrorCodes.InvalidPassword, errors);
        }

        // Add default role AFTER successful creation
        await userManager.AddToRoleAsync(user, nameof(UserRole.User));

        var profile = UserProfile.Create(user.Id, username, user.Id);
        if (firstName is not null || lastName is not null)
            profile.SetName(firstName, lastName, user.Id);
        session.Store(profile);

        user.UserProfileId = profile.Id;
        await userManager.UpdateAsync(user);

        var accessToken = jwtService.GenerateAccessToken(user.Id, email, [nameof(UserRole.User)]);
        var refreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

        await userManager.UpdateAsync(user);

        await session.SaveChangesAsync(ct);

        return AuthResult.Ok(accessToken, refreshToken,
            DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            user.Id, email, username);
    }

    public async Task<AuthResult> LoginAsync(
        string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return AuthResult.Failure(ErrorCodes.InvalidCredentials, "Invalid email or password.");

        var validPassword = await userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            return AuthResult.Failure(ErrorCodes.InvalidCredentials, "Invalid email or password.");

        var profile = user.UserProfileId.HasValue
            ? await session.LoadAsync<UserProfile>(user.UserProfileId.Value, ct)
            : null;

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtService.GenerateAccessToken(user.Id, email, roles);
        var refreshToken = jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

        await userManager.UpdateAsync(user);

        return AuthResult.Ok(accessToken, refreshToken,
            DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            user.Id, email, profile?.DisplayName ?? email);
    }

    public async Task<AuthResult> RefreshTokenAsync(
        string accessToken, string refreshToken, CancellationToken ct = default)
    {
        var user = await EntityFrameworkQueryableExtensions
            .FirstOrDefaultAsync(userManager.Users, u => u.RefreshToken == refreshToken, ct);
        if (user is null || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
            return AuthResult.Failure(ErrorCodes.InvalidRefreshToken, "Invalid or expired refresh token.");

        var profile = user.UserProfileId.HasValue
            ? await session.LoadAsync<UserProfile>(user.UserProfileId.Value, ct)
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
    }

}
