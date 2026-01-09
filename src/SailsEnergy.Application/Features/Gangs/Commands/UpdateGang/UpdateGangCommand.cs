namespace SailsEnergy.Application.Features.Gangs.Commands.UpdateGang;

public record UpdateGangCommand(
    Guid GangId,
    string? Name,
    string? Description);
