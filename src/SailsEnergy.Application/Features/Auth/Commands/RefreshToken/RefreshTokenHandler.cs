using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Telemetry;

namespace SailsEnergy.Application.Features.Auth.Commands.RefreshToken;

public static class RefreshTokenHandler
{
    public static async Task<AuthResult> HandleAsync(
        RefreshTokenCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Auth.StartActivity("RefreshToken");

        var result = await authService.RefreshTokenAsync(command, ct);

        activity?.SetTag("success", result.IsSuccess);
        if (result.IsSuccess)
            activity?.SetTag("user.id", result.UserId?.ToString());

        return result;
    }
}
