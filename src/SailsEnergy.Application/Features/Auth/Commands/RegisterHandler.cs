using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Auth.Commands;

public static class RegisterHandler
{
    public static async Task<AuthResponse> HandleAsync(
        RegisterCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.RegisterAsync(
            command.Email, command.Password, command.ConfirmPassword, command.DisplayName,
            command.FirstName, command.LastName, ct);

        return !result.Success
            ? throw new BusinessRuleException(result.ErrorCode!, result.ErrorMessage!)
            : new AuthResponse(
                result.AccessToken!, result.RefreshToken!, result.ExpiresAt!.Value,
                result.UserId!.Value, result.Email!, result.Username!);
    }
}
