using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.EnergyLogs.Commands.DeleteEnergyLog;

public static class DeleteEnergyLogHandler
{
    public static async Task HandleAsync(
        DeleteEnergyLogCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ILogger<DeleteEnergyLogCommand> logger,
        CancellationToken ct)
    {
        using var activity = ActivitySources.EnergyLogs.StartActivity("DeleteEnergyLog");

        activity?.SetTag("log.id", command.LogId.ToString());

        var userId = currentUser.UserId!.Value;

        var log = await dbContext.EnergyLogs
            .FirstOrDefaultAsync(l => l.Id == command.LogId && !l.IsDeleted, ct);
        if (log is null)
            throw new BusinessRuleException(ErrorCodes.NotFound, "Energy log not found.");

        var member = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == log.GangId && m.UserId == userId && !m.IsActive, ct);

        var isAdmin = member?.Role is MemberRole.Owner or MemberRole.Admin;
        if (log.LoggedByUserId != userId && !isAdmin)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "You can only delete your own logs.");

        if (log.LoggedByUserId == userId && !isAdmin)
        {
            var editWindow = TimeSpan.FromMinutes(5);
            if (DateTimeOffset.UtcNow - log.CreatedAt > editWindow)
                throw new BusinessRuleException("EDIT_WINDOW_EXPIRED", "You can only delete logs within 5 minutes.");
        }

        log.SoftDelete(userId);

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} deleted energy log {LogId}", userId, command.LogId);
    }
}
