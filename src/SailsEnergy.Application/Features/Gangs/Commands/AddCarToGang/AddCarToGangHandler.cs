using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands.AddCarToGang;

public static class AddCarToGangHandler
{
    public static async Task HandleAsync(
        AddCarToGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        CancellationToken ct)
    {
        var gang = await dbContext.Gangs.FindAsync([command.GangId], ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Gang not found.");

        var car = await dbContext.Cars.FindAsync([command.CarId], ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Car not found.");

        if (car.OwnerId != currentUser.UserId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "You can only add your own cars.");

        var exists = await dbContext.GangCars
            .AnyAsync(gc => gc.GangId == command.GangId && gc.CarId == command.CarId, ct);
        if (exists)
            throw new BusinessRuleException(ErrorCodes.Conflict, "Car already in gang.");

        var gangCar = GangCar.Create(command.GangId, command.CarId, currentUser.UserId!.Value);
        dbContext.GangCars.Add(gangCar);
        await dbContext.SaveChangesAsync(ct);
        await cache.RemoveAsync(CacheKeys.GangCars(command.GangId), ct);
    }
}
