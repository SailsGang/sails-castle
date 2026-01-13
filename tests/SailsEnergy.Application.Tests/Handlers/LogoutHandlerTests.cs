using NSubstitute;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Auth.Commands.Logout;

namespace SailsEnergy.Application.Tests.Handlers;

public class LogoutHandlerTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

    [Fact]
    public async Task HandleAsync_WithAuthenticatedUser_CallsLogout()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new LogoutCommand();

        _currentUser.UserId.Returns(userId);
        _authService
            .LogoutAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await LogoutHandler.HandleAsync(
            command,
            _authService,
            _currentUser,
            CancellationToken.None);

        // Assert
        await _authService.Received(1).LogoutAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithNoUser_ThrowsUnauthorized()
    {
        // Arrange
        var command = new LogoutCommand();
        _currentUser.UserId.Returns((Guid?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            LogoutHandler.HandleAsync(
                command,
                _authService,
                _currentUser,
                CancellationToken.None));
    }
}
