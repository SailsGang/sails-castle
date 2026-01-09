namespace SailsEnergy.Application.Features.Gangs.Commands.RemoveMember;

public record RemoveMemberCommand(Guid GangId, Guid MemberId);
