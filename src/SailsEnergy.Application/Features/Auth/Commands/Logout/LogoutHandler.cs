using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Telemetry;

namespace SailsEnergy.Application.Features.Auth.Commands.Logout;

public static class LogoutHandler
{
    public static async Task HandleAsync(
        LogoutCommand command,
        IAuthService authService,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Auth.StartActivity("Logout");

        if (!currentUser.UserId.HasValue)
            throw new UnauthorizedAccessException();

        activity?.SetTag("user.id", currentUser.UserId.Value.ToString());

        await authService.LogoutAsync(currentUser.UserId.Value, ct);
    }
}
