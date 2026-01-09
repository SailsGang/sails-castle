using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Cars.Responses;

namespace SailsEnergy.Application.Features.Cars.Queries.GetCarById;

public static class GetCarByIdHandler
{
    public static async Task<CarResponse?> HandleAsync(
        GetCarByIdQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var car = await dbContext.Cars
            .AsNoTracking()
            .Where(c => c.Id == query.CarId && c.OwnerId == currentUser.UserId)
            .Select(c => new CarResponse(
                c.Id,
                c.Name,
                c.Model,
                c.Manufacturer,
                c.LicensePlate,
                c.BatteryCapacityKwh,
                c.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return car;
    }
}
