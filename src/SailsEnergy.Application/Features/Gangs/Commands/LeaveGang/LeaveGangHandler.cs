using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands.LeaveGang;

public static class LeaveGangHandler
{
    public static async Task HandleAsync(
        LeaveGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Members.StartActivity("LeaveGang");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());
        var member = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == command.GangId && m.UserId == currentUser.UserId, ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "You are not a member of this gang.");

        var gang = await dbContext.Gangs.FindAsync([command.GangId], ct);
        if (gang?.OwnerId == currentUser.UserId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Owner cannot leave. Transfer ownership first.");

        member.Deactivate(currentUser.UserId!.Value);
        await dbContext.SaveChangesAsync(ct);

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.MemberLeft,
            new MemberLeftPayload(command.GangId, currentUser.UserId!.Value, DateTimeOffset.UtcNow),
            ct);

        await cache.RemoveAsync(CacheKeys.GangMembers(command.GangId), ct);
        await cache.RemoveAsync(CacheKeys.UserGangs(currentUser.UserId!.Value), ct);
    }
}
