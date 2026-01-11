using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Domain.Events;

namespace SailsEnergy.Application.Features.EnergyLogs.EventHandlers;

public static class EnergyLoggedEventHandler
{
    public static async Task HandleAsync(
        EnergyLoggedEvent @event,
        IRealtimeNotificationService notificationService,
        ILogger<EnergyLoggedEvent> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Broadcasting energy logged event for gang {GangId}", @event.GangId);
        var payload = new EnergyLoggedPayload(
            @event.GangId,
            @event.PeriodId,
            @event.EnergyLogId,
            @event.EnergyKwh,
            DateTimeOffset.UtcNow);

        await notificationService.NotifyGangAsync(
            @event.GangId,
            NotificationEvents.EnergyLogged,
            payload,
            ct);
    }
}
