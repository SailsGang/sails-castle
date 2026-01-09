using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Auth.Commands.Login;
using SailsEnergy.Application.Features.Auth.Commands.RefreshToken;
using SailsEnergy.Application.Features.Auth.Commands.Register;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Infrastructure.Identity;

namespace SailsEnergy.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly IAppDbContext _dbContext;
    private readonly IAuditService _auditService;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var userStore = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            userStore, null, null, null, null, null, null, null, null);

        _jwtService = Substitute.For<IJwtService>();
        _jwtSettings = Options.Create(new JwtSettings
        {
            Secret = "test-secret-key-32-chars-minimum!",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        });
        _dbContext = Substitute.For<IAppDbContext>();
        _auditService = Substitute.For<IAuditService>();

        // Setup mock DbSet for UserProfiles
        var emptyProfiles = new List<UserProfile>().AsQueryable();
        var mockSet = Substitute.For<DbSet<UserProfile>, IQueryable<UserProfile>>();
        ((IQueryable<UserProfile>)mockSet).Provider.Returns(emptyProfiles.Provider);
        ((IQueryable<UserProfile>)mockSet).Expression.Returns(emptyProfiles.Expression);
        ((IQueryable<UserProfile>)mockSet).ElementType.Returns(emptyProfiles.ElementType);
        ((IQueryable<UserProfile>)mockSet).GetEnumerator().Returns(emptyProfiles.GetEnumerator());
        _dbContext.UserProfiles.Returns(mockSet);

        _sut = new AuthService(_userManager, _jwtService, _jwtSettings, _dbContext, _auditService);
    }

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123!", "Password123!", "testuser", null, null);

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), command.Password)
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.UpdateAsync(Arg.Any<ApplicationUser>())
            .Returns(IdentityResult.Success);
        _jwtService.GenerateAccessToken(Arg.Any<Guid>(), command.Email, Arg.Any<IEnumerable<string>>())
            .Returns("access-token");
        _jwtService.GenerateRefreshToken().Returns("refresh-token");

        // Act
        var result = await _sut.RegisterAsync(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.Email.Should().Be(command.Email);
        result.DisplayName.Should().Be(command.DisplayName);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var command = new RegisterCommand("existing@example.com", "Password123!", "Password123!", "testuser", null, null);
        var existingUser = new ApplicationUser { Email = command.Email };

        _userManager.FindByEmailAsync(command.Email).Returns(existingUser);

        // Act
        var result = await _sut.RegisterAsync(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.EmailExists);
    }

    [Fact]
    public async Task RegisterAsync_WithMismatchedPasswords_ShouldReturnFailure()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123!", "DifferentPassword!", "testuser", null, null);

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);

        // Act
        var result = await _sut.RegisterAsync(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ValidationFailed);
    }

    [Fact]
    public async Task RegisterAsync_WithWeakPassword_ShouldReturnFailure()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "weak", "weak", "testuser", null, null);

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), command.Password)
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        // Act
        var result = await _sut.RegisterAsync(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidPassword);
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand("nonexistent@example.com", "Password123!");

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);

        // Act
        var result = await _sut.LoginAsync(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidCredentials);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldReturnFailure()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword!");
        var user = new ApplicationUser { Email = command.Email };

        _userManager.FindByEmailAsync(command.Email).Returns(user);
        _userManager.CheckPasswordAsync(user, command.Password).Returns(false);

        // Act
        var result = await _sut.LoginAsync(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidCredentials);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact(Skip = "Requires integration testing - EF Core FirstOrDefaultAsync cannot be mocked with NSubstitute")]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldReturnFailure()
    {
        var command = new RefreshTokenCommand("access-token", "invalid-refresh-token");

        var result = await _sut.RefreshTokenAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidRefreshToken);
    }

    #endregion

    #region LogoutAsync Tests

    [Fact]
    public async Task LogoutAsync_WithValidUser_ShouldClearRefreshToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            RefreshToken = "some-token",
            RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        _userManager.FindByIdAsync(userId.ToString()).Returns(user);
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);

        // Act
        await _sut.LogoutAsync(userId);

        // Assert
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiryTime.Should().BeNull();
        await _userManager.Received(1).UpdateAsync(user);
    }

    [Fact]
    public async Task LogoutAsync_WithNonExistentUser_ShouldNotThrow()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userManager.FindByIdAsync(userId.ToString()).Returns((ApplicationUser?)null);

        // Act
        var act = async () => await _sut.LogoutAsync(userId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion
}
