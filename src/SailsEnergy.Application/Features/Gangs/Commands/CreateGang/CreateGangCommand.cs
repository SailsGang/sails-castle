using System.ComponentModel.DataAnnotations;

namespace SailsEnergy.Application.Features.Gangs.Commands.CreateGang;

public record CreateGangCommand(
    [Required] string Name,
    string? Description);
