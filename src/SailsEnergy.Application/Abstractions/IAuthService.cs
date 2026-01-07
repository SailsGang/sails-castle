using SailsEnergy.Application.Features.Auth.Commands;

namespace SailsEnergy.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterCommand command, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginCommand command, CancellationToken ct = default);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken ct = default);
    Task LogoutAsync(Guid userId, CancellationToken ct = default);
}
