using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.RemoveMember;

public static class RemoveMemberHandler
{
    public static async Task HandleAsync(
        RemoveMemberCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<RemoveMemberCommand> logger,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Members.StartActivity("RemoveMember");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("member.id", command.MemberId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());
        var member = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == command.GangId && m.Id == command.MemberId, ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Member not found.");

        var currentMembership = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == command.GangId && m.UserId == currentUser.UserId, ct);

        if (currentMembership is null ||
            (currentMembership.Role != MemberRole.Owner && currentMembership.Role != MemberRole.Admin))
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Only Owner or Admin can remove members.");

        if (member.Role == MemberRole.Owner)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Cannot remove the owner.");

        logger.LogInformation("User {CurrentUserId} removing member {MemberId} from gang {GangId}",
            currentUser.UserId, command.MemberId, command.GangId);

        var removedUserId = member.UserId;
        member.Deactivate(currentUser.UserId!.Value);
        await dbContext.SaveChangesAsync(ct);

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.MemberKicked,
            new MemberLeftPayload(command.GangId, removedUserId, DateTimeOffset.UtcNow),
            ct);

        await notificationService.SendToUserAsync(
            removedUserId,
            NotificationEvents.MemberKicked,
            new { GangId = command.GangId },
            ct);

        await cache.RemoveAsync(CacheKeys.GangMembers(command.GangId), ct);
        await cache.RemoveAsync(CacheKeys.UserGangs(removedUserId), ct);

        logger.LogInformation("Member {MemberId} removed from gang {GangId}", command.MemberId, command.GangId);
    }
}
