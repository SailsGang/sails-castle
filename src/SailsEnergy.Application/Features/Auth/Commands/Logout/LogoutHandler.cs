using SailsEnergy.Application.Abstractions;

namespace SailsEnergy.Application.Features.Auth.Commands.Logout;

public static class LogoutHandler
{
    public static async Task HandleAsync(
        LogoutCommand command,
        IAuthService authService,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        if (!currentUser.UserId.HasValue)
            throw new UnauthorizedAccessException();

        await authService.LogoutAsync(currentUser.UserId.Value, ct);
    }
}
