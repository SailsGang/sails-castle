using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Cars.Responses;

namespace SailsEnergy.Application.Features.Cars.Queries.GetMyCars;

public static class GetMyCarsHandler
{
    public static async Task<PaginatedResponse<CarResponse>> HandleAsync(
        GetMyCarsQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var baseQuery = dbContext.Cars
            .AsNoTracking()
            .Where(c => c.OwnerId == currentUser.UserId)
            .Select(c => new CarResponse(
                c.Id,
                c.Name,
                c.Model,
                c.Manufacturer,
                c.LicensePlate,
                c.BatteryCapacityKwh,
                c.CreatedAt));

        var totalCount = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResponse<CarResponse>(items, totalCount, page, pageSize);
    }
}
