namespace SailsEnergy.Application.Features.Auth.Commands;

/// <summary>
/// Success response for authentication operations
/// </summary>
/// <param name="AccessToken">JWT access token for API authorization</param>
/// <param name="RefreshToken">Token to obtain new access tokens</param>
/// <param name="ExpiresAt">Access token expiration timestamp</param>
/// <param name="UserId">Authenticated user's unique identifier</param>
/// <param name="Email">User's email address</param>
/// <param name="DisplayName">User's display name</param>
public record AuthSuccessResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    string Email,
    string DisplayName);
