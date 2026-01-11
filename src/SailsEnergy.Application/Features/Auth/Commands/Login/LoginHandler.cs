using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Telemetry;

namespace SailsEnergy.Application.Features.Auth.Commands.Login;

public static class LoginHandler
{
    public static async Task<AuthResult> HandleAsync(
        LoginCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Auth.StartActivity("Login");
        activity?.SetTag("email", command.Email);

        var result = await authService.LoginAsync(command, ct);

        activity?.SetTag("success", result.IsSuccess);
        if (result.IsSuccess)
            activity?.SetTag("user.id", result.UserId?.ToString());

        return result;
    }
}
