namespace SailsEnergy.Application.Features.Auth.Commands;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    string Email,
    string DisplayName);
