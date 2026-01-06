namespace SailsEnergy.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken);
