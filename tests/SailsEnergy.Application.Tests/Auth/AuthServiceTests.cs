using FluentAssertions;
using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Common;
using SailsEnergy.Infrastructure.Identity;

namespace SailsEnergy.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly IDocumentSession _documentSession;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        // Mock UserManager
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
        _documentSession = Substitute.For<IDocumentSession>();

        _sut = new AuthService(_userManager, _jwtService, _jwtSettings, _documentSession);
    }

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var username = "testuser";

        _userManager.FindByEmailAsync(email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), password)
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.UpdateAsync(Arg.Any<ApplicationUser>())
            .Returns(IdentityResult.Success);
        _jwtService.GenerateAccessToken(Arg.Any<Guid>(), email, Arg.Any<IEnumerable<string>>())
            .Returns("access-token");
        _jwtService.GenerateRefreshToken().Returns("refresh-token");

        // Act
        var result = await _sut.RegisterAsync(email, password, password, username, null, null);

        // Assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.Email.Should().Be(email);
        result.Username.Should().Be(username);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var email = "existing@example.com";
        var existingUser = new ApplicationUser { Email = email };

        _userManager.FindByEmailAsync(email).Returns(existingUser);

        // Act
        var result = await _sut.RegisterAsync(email, "Password123!", "Password123!", "testuser", null, null);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.EmailExists);
    }

    [Fact]
    public async Task RegisterAsync_WithMismatchedPasswords_ShouldReturnFailure()
    {
        // Arrange
        var email = "test@example.com";
        _userManager.FindByEmailAsync(email).Returns((ApplicationUser?)null);

        // Act
        var result = await _sut.RegisterAsync(email, "Password123!", "DifferentPassword!", "testuser", null, null);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.ValidationFailed);
        result.ErrorMessage.Should().Contain("Passwords do not match");
    }

    [Fact]
    public async Task RegisterAsync_WhenCreateFails_ShouldReturnFailure()
    {
        // Arrange
        var email = "test@example.com";
        _userManager.FindByEmailAsync(email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        // Act
        var result = await _sut.RegisterAsync(email, "weak", "weak", "testuser", null, null);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidPassword);
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = email, UserName = "testuser" };

        _userManager.FindByEmailAsync(email).Returns(user);
        _userManager.CheckPasswordAsync(user, password).Returns(true);
        _userManager.GetRolesAsync(user).Returns(new List<string> { "User" });
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);
        _jwtService.GenerateAccessToken(user.Id, email, Arg.Any<IEnumerable<string>>())
            .Returns("access-token");
        _jwtService.GenerateRefreshToken().Returns("refresh-token");

        // Act
        var result = await _sut.LoginAsync(email, password);

        // Assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnFailure()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _userManager.FindByEmailAsync(email).Returns((ApplicationUser?)null);

        // Act
        var result = await _sut.LoginAsync(email, "Password123!");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidCredentials);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldReturnFailure()
    {
        // Arrange
        var email = "test@example.com";
        var user = new ApplicationUser { Email = email };

        _userManager.FindByEmailAsync(email).Returns(user);
        _userManager.CheckPasswordAsync(user, Arg.Any<string>()).Returns(false);

        // Act
        var result = await _sut.LoginAsync(email, "WrongPassword!");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCodes.InvalidCredentials);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact(Skip = "Requires integration testing - EF Core FirstOrDefaultAsync cannot be mocked with NSubstitute")]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldReturnFailure()
    {
        // This test requires integration testing with a real EF Core DbContext
        // because EntityFrameworkQueryableExtensions.FirstOrDefaultAsync cannot be mocked
        // See Phase 10: Testing & Polish for integration tests

        // Act
        var result = await _sut.RefreshTokenAsync("access-token", "invalid-refresh-token");

        // Assert
        result.Success.Should().BeFalse();
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
