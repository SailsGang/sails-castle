using System.ComponentModel.DataAnnotations;

namespace SailsEnergy.Application.Features.Auth.Commands;

public record RefreshTokenCommand(
    [Required] string AccessToken,
    [Required] string RefreshToken);
