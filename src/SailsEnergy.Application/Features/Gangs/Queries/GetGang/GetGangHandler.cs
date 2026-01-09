using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Gangs.Responses;

namespace SailsEnergy.Application.Features.Gangs.Queries.GetGang;

public static class GetGangHandler
{
    public static async Task<GangResponse?> HandleAsync(
        GetGangQuery query,
        IAppDbContext dbContext,
        ICacheService cache,
        CancellationToken ct)
    {
        var cached = await cache.GetEntityAsync<GangResponse>(query.GangId, ct);
        if (cached is not null)
            return cached;

        var gang = await dbContext.Gangs
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == query.GangId, ct);

        if (gang is null)
            return null;

        var response = new GangResponse(
            gang.Id, gang.Name, gang.Description, gang.OwnerId, gang.CreatedAt);

        await cache.SetEntityAsync(query.GangId, response, ct);

        return response;
    }
}
