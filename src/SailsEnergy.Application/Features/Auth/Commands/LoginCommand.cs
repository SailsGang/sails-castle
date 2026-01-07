using System.ComponentModel.DataAnnotations;

namespace SailsEnergy.Application.Features.Auth.Commands;

public record LoginCommand(
    [Required] string Email,
    [Required] string Password);
