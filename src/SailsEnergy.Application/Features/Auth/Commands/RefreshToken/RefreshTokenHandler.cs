using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;

namespace SailsEnergy.Application.Features.Auth.Commands.RefreshToken;

public static class RefreshTokenHandler
{
    public static async Task<AuthResult> HandleAsync(
        RefreshTokenCommand command,
        IAuthService authService,
        CancellationToken ct) => await authService.RefreshTokenAsync(command, ct);
}
