using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.EnergyLogs.Responses;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.EnergyLogs.Commands.LogEnergy;

public static class LogEnergyHandler
{
    public static async Task<LogEnergyResponse> HandleAsync(
        LogEnergyCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<LogEnergyCommand> logger,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.EnergyLogs.StartActivity("LogEnergy");

        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("energy.kwh", command.EnergyKwh);

        var userId = currentUser.UserId!.Value;

        activity?.SetTag("user.id", userId.ToString());

        var isMember = await dbContext.GangMembers
            .AnyAsync(m => m.GangId == command.GangId && m.UserId == userId && m.IsActive, ct);
        if (!isMember)
            throw new BusinessRuleException("NOT_MEMBER", "You are not a member of this gang.");

        var activePeriod = await dbContext.Periods
            .FirstOrDefaultAsync(p => p.GangId == command.GangId && p.Status == PeriodStatus.Active, ct);
        if (activePeriod is null)
            throw new BusinessRuleException("NO_ACTIVE_PERIOD", "There is no active period in this gang.");

        activity?.SetTag("period.id", activePeriod.Id.ToString());

        var gangCar = await dbContext.GangCars
            .FirstOrDefaultAsync(gc => gc.Id == command.GangCarId && gc.GangId == command.GangId && gc.IsActive, ct);
        if (gangCar is null)
            throw new BusinessRuleException("CAR_NOT_IN_GANG", "This car is not assigned to the gang.");

        var cacheKey = $"gang:{command.GangId}:current-tariff";
        var tariff = await cache.GetOrCreateAsync(
            cacheKey,
            async () => await dbContext.Tariffs
                .Where(t => t.GangId == command.GangId)
                .OrderByDescending(t => t.EffectiveFrom)
                .FirstOrDefaultAsync(ct),
            TimeSpan.FromMinutes(5),
            ct);
        if (tariff is null)
            throw new BusinessRuleException("NO_TARIFF", "No tariff has been set for this gang.");

        activity?.SetTag("tariff.id", tariff.Id.ToString());
        activity?.SetTag("tariff.price", tariff.PricePerKwh);

        var log = EnergyLog.Create(
            command.GangId,
            command.GangCarId,
            activePeriod.Id,
            userId,
            command.EnergyKwh,
            command.ChargingDate,
            tariff.Id,
            command.Notes,
            userId);
        dbContext.EnergyLogs.Add(log);

        await dbContext.SaveChangesAsync(ct);

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.EnergyLogged,
            new EnergyLoggedPayload(command.GangId, activePeriod.Id, log.Id, command.EnergyKwh, DateTimeOffset.UtcNow),
            ct);

        var cost = command.EnergyKwh * tariff.PricePerKwh;

        activity?.SetTag("log.id", log.Id.ToString());
        activity?.SetTag("log.cost", cost);

        logger.LogInformation("User {UserId} logged {Kwh} kWh (cost: {Cost}) in gang {GangId}",
            userId, command.EnergyKwh, cost, command.GangId);

        return new LogEnergyResponse(log.Id, cost);
    }
}
