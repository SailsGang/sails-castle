using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Auth.Commands.Register;

namespace SailsEnergy.Application.Tests.Handlers;

public class RegisterHandlerTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    [Fact]
    public async Task HandleAsync_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new RegisterCommand(
            "test@example.com",
            "Password123!",
            "Password123!",
            "Test User",
            "Test",
            "User");

        var expectedResult = AuthResult.Ok(
            "access-token",
            "refresh-token",
            DateTimeOffset.UtcNow.AddHours(1),
            Guid.NewGuid(),
            "test@example.com",
            "Test User");

        _authService
            .RegisterAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await RegisterHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        await _authService.Received(1).RegisterAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterCommand(
            "existing@example.com",
            "Password123!",
            "Password123!",
            "Test User",
            "Test",
            "User");

        var expectedResult = AuthResult.Failure("DUPLICATE_EMAIL", "Email already exists");

        _authService
            .RegisterAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await RegisterHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Email already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateUsername_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterCommand(
            "new@example.com",
            "Password123!",
            "Password123!",
            "Existing User",
            "Test",
            "User");

        var expectedResult = AuthResult.Failure("DUPLICATE_USERNAME", "Username already exists");

        _authService
            .RegisterAsync(command, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await RegisterHandler.HandleAsync(
            command,
            _authService,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Username already exists", result.ErrorMessage);
    }
}
