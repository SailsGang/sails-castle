using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;

namespace SailsEnergy.Application.Features.Auth.Commands.Register;

public static class RegisterHandler
{
    public static async Task<AuthResult> HandleAsync(
        RegisterCommand command,
        IAuthService authService,
        CancellationToken ct) => await authService.RegisterAsync(command, ct);
}
