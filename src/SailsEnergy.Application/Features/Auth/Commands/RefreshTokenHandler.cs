using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Auth.Commands;

public static class RefreshTokenHandler
{
    public static async Task<AuthResponse> HandleAsync(
        RefreshTokenCommand command, IAuthService authService, CancellationToken ct)
    {
        var result = await authService.RefreshTokenAsync(
            command.AccessToken, command.RefreshToken, ct);

        return !result.Success
            ? throw new BusinessRuleException(result.ErrorCode!, result.ErrorMessage!)
            : new AuthResponse(
                result.AccessToken!, result.RefreshToken!, result.ExpiresAt!.Value,
                result.UserId!.Value, result.Email!, result.Username!);
    }
}
