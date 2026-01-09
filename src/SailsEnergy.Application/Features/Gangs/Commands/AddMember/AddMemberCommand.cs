namespace SailsEnergy.Application.Features.Gangs.Commands.AddMember;

public record AddMemberCommand(
    Guid GangId,
    Guid UserId);
