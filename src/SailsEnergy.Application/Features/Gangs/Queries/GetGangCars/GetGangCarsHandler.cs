using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;

namespace SailsEnergy.Application.Features.Gangs.Queries.GetGangCars;

public static class GetGangCarsHandler
{
    public static async Task<PaginatedResponse<GangCarResponse>> HandleAsync(
        GetGangCarsQuery query,
        IAppDbContext dbContext,
        CancellationToken ct)
    {
        var baseQuery = from gc in dbContext.GangCars.AsNoTracking()
            join c in dbContext.Cars.AsNoTracking() on gc.CarId equals c.Id
            where gc.GangId == query.GangId && !gc.IsDeleted
            select new GangCarResponse(
                gc.Id,
                gc.CarId,
                c.Name ?? c.Model,
                c.LicensePlate,
                c.BatteryCapacityKwh ?? 0,
                c.OwnerId,
                gc.CreatedAt);

        var totalCount = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .OrderBy(gc => gc.AddedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PaginatedResponse<GangCarResponse>(items, totalCount, query.Page, query.PageSize);
    }
}
