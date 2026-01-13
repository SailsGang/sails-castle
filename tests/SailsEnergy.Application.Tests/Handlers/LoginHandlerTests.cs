using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Auth.Commands.Login;

namespace SailsEnergy.Application.Tests.Handlers;

public class LoginHandlerTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ReturnsSuccessResult()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");

        var expectedResult = AuthResult.Ok(
            "access-token",
            "refresh-token",
            DateTimeOffset.UtcNow.AddHours(1),
            Guid.NewGuid(),
            "test@example.com",
            "Test User");

        _authService
            .LoginAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await LoginHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        await _authService.Received(1).LoginAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithInvalidEmail_ReturnsFailure()
    {
        // Arrange
        var command = new LoginCommand("nonexistent@example.com", "Password123!");

        var expectedResult = AuthResult.Failure("INVALID_CREDENTIALS", "Invalid credentials");

        _authService
            .LoginAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await LoginHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid credentials", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithWrongPassword_ReturnsFailure()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword!");

        var expectedResult = AuthResult.Failure("INVALID_CREDENTIALS", "Invalid credentials");

        _authService
            .LoginAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await LoginHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
