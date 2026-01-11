using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.RemoveCarFromGang;

public static class RemoveCarFromGangHandler
{
    public static async Task HandleAsync(
        RemoveCarFromGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
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

        var currentMembership = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == command.GangId && m.UserId == currentUser.UserId, ct);

        var isOwnerOrAdmin = currentMembership?.Role is MemberRole.Owner or MemberRole.Admin;
        var isCarOwner = car?.OwnerId == currentUser.UserId;

        if (!isOwnerOrAdmin && !isCarOwner)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Not authorized to remove this car.");

        gangCar.Deactivate(currentUser.UserId!.Value);
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
