using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands.RemoveCarFromGang;

public static class RemoveCarFromGangHandler
{
    public static async Task HandleAsync(
        RemoveCarFromGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Cars.StartActivity("RemoveCarFromGang");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("car.id", command.CarId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());

        var gangCar = await dbContext.GangCars
            .FirstOrDefaultAsync(gc => gc.GangId == command.GangId && gc.CarId == command.CarId, ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Car not in gang.");

        var car = await dbContext.Cars.FindAsync([command.CarId], ct);
        var isCarOwner = car?.OwnerId == currentUser.UserId;

        if (!isCarOwner)
            await gangAuth.RequireAdminAsync(command.GangId, ct);
        else
            await gangAuth.RequireMembershipAsync(command.GangId, ct);

        gangCar.SoftDelete(currentUser.UserId!.Value);
        await dbContext.SaveChangesAsync(ct);

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.CarRemovedFromGang,
            new CarRemovedFromGangPayload(
                command.GangId,
                command.CarId,
                car?.Name ?? "Unknown",
                currentUser.UserId!.Value,
                DateTimeOffset.UtcNow),
            ct);

        await cache.RemoveAsync(CacheKeys.GangCars(command.GangId), ct);
    }
}
