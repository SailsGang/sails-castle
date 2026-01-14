using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands.AddCarToGang;

public static class AddCarToGangHandler
{
    public static async Task<Guid> HandleAsync(
        AddCarToGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Cars.StartActivity("AddCarToGang");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("car.id", command.CarId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());

        await gangAuth.RequireMembershipAsync(command.GangId, ct);

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

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.CarAddedToGang,
            new CarAddedToGangPayload(
                command.GangId,
                gangCar.Id,
                command.CarId,
                car.Name ?? $"{car.Manufacturer} {car.Model}",
                currentUser.UserId!.Value,
                DateTimeOffset.UtcNow),
            ct);

        await cache.RemoveAsync(CacheKeys.GangCars(command.GangId), ct);

        return gangCar.Id;
    }
}
