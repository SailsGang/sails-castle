using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Auth.Commands;

public static class LoginHandler
{
    public static async Task<AuthResult> HandleAsync(
        LoginCommand command,
        IAuthService authService,
        CancellationToken ct) => await authService.LoginAsync(command, ct);
}
