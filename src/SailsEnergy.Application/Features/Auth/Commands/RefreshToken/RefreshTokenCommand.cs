using System.ComponentModel.DataAnnotations;

namespace SailsEnergy.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    [Required] string AccessToken,
    [Required] string RefreshToken);
