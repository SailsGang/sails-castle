using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.LeaveGang;

public static class LeaveGangHandler
{
    public static async Task HandleAsync(
        LeaveGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Members.StartActivity("LeaveGang");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());

        var member = await gangAuth.RequireMembershipAsync(command.GangId, ct);

        if (member.Role == MemberRole.Owner)
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
