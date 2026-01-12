using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;

namespace SailsEnergy.Application.Features.Gangs.Queries.GetGangMembers;

public static class GetGangMembersHandler
{
    public static async Task<PaginatedResponse<GangMemberResponse>> HandleAsync(
        GetGangMembersQuery query,
        IAppDbContext dbContext,
        CancellationToken ct)
    {
        var baseQuery = from m in dbContext.GangMembers.AsNoTracking()
            join p in dbContext.UserProfiles.AsNoTracking() on m.UserId equals p.IdentityId into profiles
            from profile in profiles.DefaultIfEmpty()
            where m.GangId == query.GangId && m.IsActive
            select new GangMemberResponse(
                m.Id,
                m.UserId,
                profile != null ? profile.DisplayName : "Unknown",
                "",
                m.Role.ToString(),
                m.CreatedAt);

        var totalCount = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .OrderBy(m => m.JoinedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PaginatedResponse<GangMemberResponse>(items, totalCount, query.Page, query.PageSize);
    }
}
