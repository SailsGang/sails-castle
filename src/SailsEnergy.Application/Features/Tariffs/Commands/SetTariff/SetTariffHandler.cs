using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Tariffs.Responses;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Features.Tariffs.Commands.SetTariff;

public static class SetTariffHandler
{
    public static async Task<TariffResponse> HandleAsync(
        SetTariffCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        ILogger<SetTariffCommand> logger,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Tariffs.StartActivity("SetTariff");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());
        activity?.SetTag("price", command.PricePerKwh);

        var userId = currentUser.UserId!.Value;

        await gangAuth.RequireAdminAsync(command.GangId, ct);

        logger.LogInformation("User {UserId} setting tariff for gang {GangId}: {Price} {Currency}",
            userId, command.GangId, command.PricePerKwh, command.Currency);

        var tariff = Tariff.Create(
            command.GangId,
            command.PricePerKwh,
            command.Currency,
            userId);

        dbContext.Tariffs.Add(tariff);
        await dbContext.SaveChangesAsync(ct);

        await cache.RemoveAsync(CacheKeys.GangTariff(command.GangId), ct);

        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.TariffChanged,
            new TariffChangedPayload(
                command.GangId,
                tariff.Id,
                command.PricePerKwh,
                command.Currency,
                DateTimeOffset.UtcNow),
            ct);

        activity?.SetTag("tariff.id", tariff.Id.ToString());
        logger.LogInformation("Tariff {TariffId} set for gang {GangId}", tariff.Id, command.GangId);

        return new TariffResponse(
            tariff.Id,
            tariff.GangId,
            tariff.PricePerKwh,
            tariff.Currency,
            tariff.EffectiveFrom,
            tariff.SetByUserId);
    }
}
