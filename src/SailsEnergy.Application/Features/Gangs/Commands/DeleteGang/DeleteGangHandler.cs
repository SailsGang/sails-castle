using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Gangs.Responses;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands.DeleteGang;

public static class DeleteGangHandler
{
    public static async Task HandleAsync(
        DeleteGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        ILogger<DeleteGangCommand> logger,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Gangs.StartActivity("DeleteGang");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());

        await gangAuth.RequireOwnerAsync(command.GangId, ct);

        var gang = await dbContext.Gangs.FindAsync([command.GangId], ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Gang not found.");

        logger.LogInformation("User {UserId} deleting gang {GangId}", currentUser.UserId, command.GangId);

        var memberUserIds = await dbContext.GangMembers
            .Where(m => m.GangId == command.GangId && m.IsActive)
            .Select(m => m.UserId)
            .ToListAsync(ct);

        gang.SoftDelete(currentUser.UserId!.Value);

        var auditLog = AuditLog.Create(
            AuditActions.GangDeleted,
            nameof(Gang),
            command.GangId,
            currentUser.UserId!.Value,
            $"Gang '{gang.Name}' deleted with {memberUserIds.Count} members");
        dbContext.AuditLogs.Add(auditLog);

        await dbContext.SaveChangesAsync(ct);

        await cache.InvalidateEntityAsync<GangResponse>(command.GangId, ct);
        await cache.RemoveAsync(CacheKeys.GangMembers(command.GangId), ct);
        await cache.RemoveAsync(CacheKeys.GangCars(command.GangId), ct);

        foreach (var memberId in memberUserIds)
        {
            await notificationService.SendToUserAsync(
                memberId,
                NotificationEvents.GangDeleted,
                new GangDeletedPayload(command.GangId, gang.Name, DateTimeOffset.UtcNow),
                ct);
        }

        logger.LogInformation("Gang {GangId} deleted successfully", command.GangId);
    }
}
