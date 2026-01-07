using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Auth.Commands;

public static class RegisterHandler
{
    public static async Task<AuthResult> HandleAsync(
        RegisterCommand command,
        IAuthService authService,
        CancellationToken ct) => await authService.RegisterAsync(command, ct);
}
