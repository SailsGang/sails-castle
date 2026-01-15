using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Settings;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Gangs.Commands.AddMember;

public static class AddMemberHandler
{
    public static async Task HandleAsync(
        AddMemberCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        IOptions<GangSettings> gangSettings,
        ILogger<AddMemberCommand> logger,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Members.StartActivity("AddMember");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());

        var gangExists = await dbContext.Gangs.AnyAsync(g => g.Id == command.GangId, ct);
        if (!gangExists)
            throw new BusinessRuleException(ErrorCodes.NotFound, "Gang not found.");

        var currentMemberCount = await dbContext.GangMembers
            .CountAsync(m => m.GangId == command.GangId && m.IsActive, ct);
        if (currentMemberCount >= gangSettings.Value.MaxMembersPerGang)
            throw new BusinessRuleException("MEMBER_LIMIT_REACHED",
                $"Gang has reached the maximum of {gangSettings.Value.MaxMembersPerGang} members.");

        var memberships = await dbContext.GangMembers
            .IgnoreQueryFilters()
            .Where(m => m.GangId == command.GangId &&
                       (m.UserId == currentUser.UserId || m.UserId == command.UserId))
            .ToListAsync(ct);

        var currentMembership = memberships
            .FirstOrDefault(m => m.UserId == currentUser.UserId && m.IsActive);

        if (currentMembership is null ||
            (currentMembership.Role != MemberRole.Owner && currentMembership.Role != MemberRole.Admin))
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Only Owner or Admin can add members.");

        var existingMember = memberships.FirstOrDefault(m => m.UserId == command.UserId);

        if (existingMember is not null && existingMember.IsActive)
            throw new BusinessRuleException(ErrorCodes.Conflict, "User is already a member.");

        logger.LogInformation("User {CurrentUserId} adding user {UserId} to gang {GangId}",
            currentUser.UserId, command.UserId, command.GangId);

        if (existingMember is not null)
            existingMember.Reactivate(currentUser.UserId!.Value);
        else
        {
            var member = GangMember.Create(command.GangId, command.UserId, MemberRole.Member, currentUser.UserId!.Value);
            dbContext.GangMembers.Add(member);
        }

        var userProfile = await dbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.IdentityId == command.UserId, ct);
        var displayName = userProfile?.DisplayName ?? "Unknown";

        await dbContext.SaveChangesAsync(ct);

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.MemberJoined,
            new MemberJoinedPayload(command.GangId, command.UserId, displayName, DateTimeOffset.UtcNow),
            ct);

        await cache.RemoveAsync(CacheKeys.GangMembers(command.GangId), ct);
        await cache.RemoveAsync(CacheKeys.UserGangs(command.UserId), ct);

        logger.LogInformation("User {UserId} added to gang {GangId}", command.UserId, command.GangId);
    }
}
