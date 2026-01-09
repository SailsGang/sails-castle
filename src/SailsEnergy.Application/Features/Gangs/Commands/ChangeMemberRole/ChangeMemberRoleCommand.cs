namespace SailsEnergy.Application.Features.Gangs.Commands.ChangeMemberRole;

public record ChangeMemberRoleCommand(Guid GangId, Guid MemberId, string Role);
