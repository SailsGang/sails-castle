using SailsEnergy.Application.Abstractions;

namespace SailsEnergy.Application.Features.Auth.Commands;

public static class LoginHandler
{
    public static async Task<AuthResult> HandleAsync(
        LoginCommand command,
        IAuthService authService,
        CancellationToken ct) => await authService.LoginAsync(command, ct);
}
