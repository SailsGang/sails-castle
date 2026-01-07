using SailsEnergy.Domain.Common;

namespace SailsEnergy.Application.Abstractions;

public record AuthTokens(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    string Email,
    string DisplayName);

public class AuthResult : Result<AuthTokens>
{
    private AuthResult(AuthTokens value) : base(value) { }

    private AuthResult(string code, string message) : base(code, message) { }

    public static AuthResult Ok(string accessToken, string refreshToken,
        DateTimeOffset expiresAt, Guid userId, string email, string displayName)
        => new(new AuthTokens(accessToken, refreshToken, expiresAt, userId, email, displayName));

    public static new AuthResult Failure(string code, string message)
        => new(code, message);
}
