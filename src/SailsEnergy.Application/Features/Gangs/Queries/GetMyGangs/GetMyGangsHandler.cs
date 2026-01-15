using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;

namespace SailsEnergy.Application.Features.Gangs.Queries.GetMyGangs;

public static class GetMyGangsHandler
{
    public static async Task<PaginatedResponse<GangListItem>> HandleAsync(
        GetMyGangsQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var baseQuery = from m in dbContext.GangMembers.AsNoTracking()
                        join g in dbContext.Gangs.AsNoTracking() on m.GangId equals g.Id
                        where m.UserId == userId && m.IsActive
                        select new GangListItem(
                            g.Id,
                            g.Name,
                            g.Description,
                            g.OwnerId,
                            m.Role.ToString(),
                            g.CreatedAt);

        var totalCount = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .OrderByDescending(g => g.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResponse<GangListItem>(items, totalCount, page, pageSize);
    }
}
