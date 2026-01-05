namespace SailsEnergy.Application.Features.Gangs.Commands;

public record CreateGangCommand(
    string Name,
    string? Description);

public record CreateGangResponse(Guid GangId);
