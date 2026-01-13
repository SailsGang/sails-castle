using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Auth.Commands.RefreshToken;

namespace SailsEnergy.Application.Tests.Handlers;

public class RefreshTokenHandlerTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    [Fact]
    public async Task HandleAsync_WithValidToken_ReturnsNewTokens()
    {
        // Arrange
        var command = new RefreshTokenCommand("old-access-token", "valid-refresh-token");

        var expectedResult = AuthResult.Ok(
            "new-access-token",
            "new-refresh-token",
            DateTimeOffset.UtcNow.AddHours(1),
            Guid.NewGuid(),
            "test@example.com",
            "Test User");

        _authService
            .RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await RefreshTokenHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task HandleAsync_WithExpiredToken_ReturnsFailure()
    {
        // Arrange
        var command = new RefreshTokenCommand("old-access-token", "expired-token");

        var expectedResult = AuthResult.Failure("TOKEN_EXPIRED", "Token expired");

        _authService
            .RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await RefreshTokenHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidToken_ReturnsFailure()
    {
        // Arrange
        var command = new RefreshTokenCommand("old-access-token", "invalid-token");

        var expectedResult = AuthResult.Failure("INVALID_TOKEN", "Invalid refresh token");

        _authService
            .RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await RefreshTokenHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
