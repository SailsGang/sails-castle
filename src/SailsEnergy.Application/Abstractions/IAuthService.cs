using SailsEnergy.Application.Features.Auth.Commands;

namespace SailsEnergy.Application.Abstractions;

/// <summary>
/// Authentication service for user registration, login, and token management
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user and returns authentication tokens
    /// </summary>
    Task<AuthResult> RegisterAsync(RegisterCommand command, CancellationToken ct = default);

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    Task<AuthResult> LoginAsync(LoginCommand command, CancellationToken ct = default);

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token
    /// </summary>
    Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken ct = default);

    /// <summary>
    /// Logs out a user by invalidating their refresh token
    /// </summary>
    Task LogoutAsync(Guid userId, CancellationToken ct = default);
}
