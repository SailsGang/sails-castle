using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;

namespace SailsEnergy.Application.Features.Gangs.Queries.GetMyGangs;

public static class GetMyGangsHandler
{
    public static async Task<List<GangListItem>> HandleAsync(
        GetMyGangsQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        var cached = await cache.GetAsync<List<GangListItem>>(CacheKeys.UserGangs(userId), ct);
        if (cached is not null)
            return cached;

        var result = await (
            from m in dbContext.GangMembers.AsNoTracking()
            join g in dbContext.Gangs.AsNoTracking() on m.GangId equals g.Id
            where m.UserId == userId && m.IsActive
            select new GangListItem(
                g.Id,
                g.Name,
                g.Description,
                g.OwnerId,
                m.Role.ToString(),
                g.CreatedAt)
        ).ToListAsync(ct);

        await cache.SetAsync(CacheKeys.UserGangs(userId), result, ct: ct);

        return result;
    }
}
