using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Telemetry;

namespace SailsEnergy.Application.Features.Auth.Commands.Register;

public static class RegisterHandler
{
    public static async Task<AuthResult> HandleAsync(
        RegisterCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Auth.StartActivity("Register");
        activity?.SetTag("email", command.Email);
        activity?.SetTag("display_name", command.DisplayName);

        var result = await authService.RegisterAsync(command, ct);

        activity?.SetTag("success", result.IsSuccess);
        if (result.IsSuccess)
            activity?.SetTag("user.id", result.UserId?.ToString());

        return result;
    }
}
