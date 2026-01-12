namespace SailsEnergy.Application.Features.Gangs.Queries.GetGangMembers;

public record GetGangMembersQuery(Guid GangId, int Page = 1, int PageSize = 50);
