namespace SailsEnergy.Application.Features.Gangs.Commands.RemoveCarFromGang;

public record RemoveCarFromGangCommand(Guid GangId, Guid CarId);
