using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Cars.Responses;

namespace SailsEnergy.Application.Features.Cars.Queries.GetMyCars;

public static class GetMyCarsHandler
{
    public static async Task<List<CarResponse>> HandleAsync(
        GetMyCarsQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var cars = await dbContext.Cars
            .AsNoTracking()
            .Where(c => c.OwnerId == currentUser.UserId)
            .Select(c => new CarResponse(
                c.Id,
                c.Name,
                c.Model,
                c.Manufacturer,
                c.LicensePlate,
                c.BatteryCapacityKwh,
                c.CreatedAt))
            .ToListAsync(ct);

        return cars;
    }
}
