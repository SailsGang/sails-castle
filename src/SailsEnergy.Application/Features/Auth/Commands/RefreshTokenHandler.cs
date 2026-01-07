using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Auth.Commands;

public static class RefreshTokenHandler
{
    public static async Task<AuthResult> HandleAsync(
        RefreshTokenCommand command,
        IAuthService authService,
        CancellationToken ct) => await authService.RefreshTokenAsync(command, ct);
}
