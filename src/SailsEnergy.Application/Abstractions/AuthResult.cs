namespace SailsEnergy.Application.Abstractions;

public record AuthResult(
    bool Success,
    string? AccessToken = null,
    string? RefreshToken = null,
    DateTimeOffset? ExpiresAt = null,
    Guid? UserId = null,
    string? Email = null,
    string? Username = null,
    string? ErrorCode = null,
    string? ErrorMessage = null)
{
    public static AuthResult Ok(string accessToken, string refreshToken,
        DateTimeOffset expiresAt, Guid userId, string email, string displayName)
        => new(true, accessToken, refreshToken, expiresAt, userId, email, displayName);
    public static AuthResult Failure(string code, string message)
        => new(false, ErrorCode: code, ErrorMessage: message);
}
