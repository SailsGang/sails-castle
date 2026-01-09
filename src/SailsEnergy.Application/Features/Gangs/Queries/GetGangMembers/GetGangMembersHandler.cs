using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;

namespace SailsEnergy.Application.Features.Gangs.Queries.GetGangMembers;

public static class GetGangMembersHandler
{
    public static async Task<List<GangMemberResponse>> HandleAsync(
        GetGangMembersQuery query,
        IAppDbContext dbContext,
        ICacheService cache,
        CancellationToken ct)
    {
        var cached = await cache.GetAsync<List<GangMemberResponse>>(CacheKeys.GangMembers(query.GangId), ct);
        if (cached is not null)
            return cached;

        var result = await (
            from m in dbContext.GangMembers.AsNoTracking()
            join p in dbContext.UserProfiles.AsNoTracking() on m.UserId equals p.IdentityId into profiles
            from profile in profiles.DefaultIfEmpty()
            where m.GangId == query.GangId
            select new GangMemberResponse(
                m.Id,
                m.UserId,
                profile != null ? profile.DisplayName : "Unknown",
                "",
                m.Role.ToString(),
                m.CreatedAt)
        ).ToListAsync(ct);

        await cache.SetAsync(CacheKeys.GangMembers(query.GangId), result, ct: ct);

        return result;
    }
}
