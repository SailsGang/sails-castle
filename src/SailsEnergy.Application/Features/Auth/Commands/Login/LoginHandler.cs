using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;

namespace SailsEnergy.Application.Features.Auth.Commands.Login;

public static class LoginHandler
{
    public static async Task<AuthResult> HandleAsync(
        LoginCommand command,
        IAuthService authService,
        CancellationToken ct) => await authService.LoginAsync(command, ct);
}
