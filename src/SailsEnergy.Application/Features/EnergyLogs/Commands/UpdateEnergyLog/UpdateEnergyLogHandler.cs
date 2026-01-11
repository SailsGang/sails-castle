using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.EnergyLogs.Commands.UpdateEnergyLog;

public static class UpdateEnergyLogHandler
{
    public static async Task HandleAsync(
        UpdateEnergyLogCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ILogger<UpdateEnergyLogCommand> logger,
        CancellationToken ct)
    {
        using var activity = ActivitySources.EnergyLogs.StartActivity("UpdateEnergyLog");

        activity?.SetTag("log.id", command.LogId.ToString());
        activity?.SetTag("user.id", currentUser.UserId.ToString());

        var userId = currentUser.UserId!.Value;

        var log = await dbContext.EnergyLogs
            .FirstOrDefaultAsync(l => l.Id == command.LogId, ct);
        if (log is null)
            throw new BusinessRuleException(ErrorCodes.NotFound, "Energy log not found.");

        if (log.LoggedByUserId != userId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "You can only edit your own logs.");

        if (command.EnergyKwh.HasValue)
            log.SetEnergyKwh(command.EnergyKwh.Value, userId);
        if (command.ChargingDate.HasValue)
            log.SetChargingDate(command.ChargingDate.Value, userId);
        if (command.Notes is not null)
            log.SetNotes(command.Notes, userId);

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} updated energy log {LogId}", userId, command.LogId);
    }
}
