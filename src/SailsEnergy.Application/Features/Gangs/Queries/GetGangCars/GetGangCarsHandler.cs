using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;

namespace SailsEnergy.Application.Features.Gangs.Queries.GetGangCars;

public static class GetGangCarsHandler
{
    public static async Task<List<GangCarResponse>> HandleAsync(
        GetGangCarsQuery query,
        IAppDbContext dbContext,
        ICacheService cache,
        CancellationToken ct)
    {
        var cached = await cache.GetAsync<List<GangCarResponse>>(CacheKeys.GangCars(query.GangId), ct);
        if (cached is not null)
            return cached;

        var result = await (
            from gc in dbContext.GangCars.AsNoTracking()
            join c in dbContext.Cars.AsNoTracking() on gc.CarId equals c.Id
            where gc.GangId == query.GangId
            select new GangCarResponse(
                gc.Id,
                gc.CarId,
                c.Name ?? c.Model,
                c.LicensePlate,
                c.BatteryCapacityKwh ?? 0,
                c.OwnerId,
                gc.CreatedAt)
        ).ToListAsync(ct);

        await cache.SetAsync(CacheKeys.GangCars(query.GangId), result, ct: ct);

        return result;
    }
}
