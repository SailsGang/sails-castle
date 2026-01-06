using SailsEnergy.Application.Features.Auth.Commands;

namespace SailsEnergy.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(
        string email,
        string password,
        string confirmPassword,
        string username,
        string? firstName,
        string? lastName,
        CancellationToken ct = default);

    Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken ct = default);

    Task<AuthResult> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        CancellationToken ct = default);

    Task LogoutAsync(Guid userId, CancellationToken ct = default);
}
