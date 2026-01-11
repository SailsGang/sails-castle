using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.ChangeMemberRole;

public static class ChangeMemberRoleHandler
{
    public static async Task HandleAsync(
        ChangeMemberRoleCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        ILogger<ChangeMemberRoleCommand> logger,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Members.StartActivity("ChangeMemberRole");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("member.id", command.MemberId.ToString());
        activity?.SetTag("new.role", command.Role);
        activity?.SetTag("user.id", currentUser.UserId?.ToString());

        await gangAuth.RequireOwnerAsync(command.GangId, ct);

        var member = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == command.GangId && m.Id == command.MemberId, ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Member not found.");

        if (member.Role == MemberRole.Owner)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Cannot change owner's role.");

        if (!Enum.TryParse<MemberRole>(command.Role, true, out var newRole))
            throw new BusinessRuleException(ErrorCodes.ValidationFailed, "Invalid role.");

        if (newRole == MemberRole.Owner)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Cannot assign owner role.");

        logger.LogInformation("User {CurrentUserId} changing role of member {MemberId} to {Role} in gang {GangId}",
            currentUser.UserId, command.MemberId, command.Role, command.GangId);

        member.SetRole(newRole, currentUser.UserId!.Value);
        await dbContext.SaveChangesAsync(ct);
        await cache.RemoveAsync(CacheKeys.GangMembers(command.GangId), ct);

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.MemberRoleChanged,
            new MemberRoleChangedPayload(command.GangId, member.UserId, command.Role, DateTimeOffset.UtcNow),
            ct);

        await notificationService.SendToUserAsync(
            member.UserId,
            NotificationEvents.MemberRoleChanged,
            new { GangId = command.GangId, NewRole = command.Role },
            ct);

        logger.LogInformation("Role changed successfully for member {MemberId} in gang {GangId}", command.MemberId, command.GangId);
    }
}
