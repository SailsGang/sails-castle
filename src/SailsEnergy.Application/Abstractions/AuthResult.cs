using System.Text.Json.Serialization;

namespace SailsEnergy.Application.Abstractions;

public class AuthResult
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public Guid? UserId { get; init; }
    public string? Email { get; init; }
    public string? DisplayName { get; init; }

    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }

    [JsonConstructor]
    public AuthResult() { }

    private AuthResult(bool isSuccess) => IsSuccess = isSuccess;

    public static AuthResult Ok(string accessToken, string refreshToken,
        DateTimeOffset expiresAt, Guid userId, string email, string displayName)
        => new(true)
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = userId,
            Email = email,
            DisplayName = displayName
        };

    public static AuthResult Failure(string code, string message)
        => new(false)
        {
            ErrorCode = code,
            ErrorMessage = message
        };
}
