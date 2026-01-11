using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Periods.Responses;
using SailsEnergy.Application.Notifications;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;
using IDocumentSession = Marten.IDocumentSession;

namespace SailsEnergy.Application.Features.Periods.Commands.ClosePeriod;

public static class ClosePeriodHandler
{
    public static async Task<ClosePeriodResponse> HandleAsync(
        ClosePeriodCommand command,
        IAppDbContext dbContext,
        IDocumentSession documentSession,
        ICurrentUserService currentUser,
        IGangAuthorizationService gangAuth,
        IPeriodReportService reportService,
        IUnitOfWorkCoordinator unitOfWork,
        ICacheService cache,
        ILogger<ClosePeriodCommand> logger,
        IRealtimeNotificationService notificationService,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Periods.StartActivity("ClosePeriod");
        activity?.SetTag("gang.id", command.GangId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());

        var userId = currentUser.UserId!.Value;

        await gangAuth.RequireAdminAsync(command.GangId, ct);

        var currentPeriod = await dbContext.Periods
            .Where(p => p.GangId == command.GangId && p.Status == PeriodStatus.Active)
            .FirstOrDefaultAsync(ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "No active period found.");

        logger.LogInformation("User {UserId} closing period {PeriodId} for gang {GangId}",
            userId, currentPeriod.Id, command.GangId);

        var tariff = await dbContext.Tariffs
            .Where(t => t.GangId == command.GangId && t.EffectiveFrom <= DateTimeOffset.UtcNow)
            .OrderByDescending(t => t.EffectiveFrom)
            .FirstOrDefaultAsync(ct);

        var pricePerKwh = tariff?.PricePerKwh ?? 0m;
        var currency = tariff?.Currency ?? "UAH";

        var report = await reportService.GenerateReportAsync(
            command.GangId,
            currentPeriod,
            pricePerKwh,
            currency,
            userId,
            ct);

        var newPeriod = await unitOfWork.ExecuteAsync(command.GangId, async context =>
        {
            currentPeriod.Close(userId);

            var period = Period.Create(command.GangId, userId);
            dbContext.Periods.Add(period);

            documentSession.Store(report);

            context.RegisterCompensation(_ =>
            {
                logger.LogWarning(
                    "Compensation triggered for ClosePeriod. Period {PeriodId} EF changes rolled back, Marten report {ReportId} may need cleanup.",
                    currentPeriod.Id, report.Id);

                return Task.CompletedTask;
            });

            await context.SaveEfChangesAsync(ct);
            await context.SaveMartenChangesAsync(ct);

            return period;
        }, ct);

        await cache.RemoveAsync(CacheKeys.GangActivePeriod(command.GangId), ct);

        var gang = await dbContext.Gangs.FindAsync([command.GangId], ct);
        await notificationService.NotifyGangAsync(
            command.GangId,
            NotificationEvents.PeriodClosed,
            new PeriodClosedPayload(
                command.GangId,
                gang?.Name ?? "Unknown",
                currentPeriod.Id,
                report.TotalEnergyKwh,
                report.TotalCost,
                currency,
                DateTimeOffset.UtcNow),
            ct);

        activity?.SetTag("period.id", currentPeriod.Id.ToString());
        activity?.SetTag("new_period.id", newPeriod.Id.ToString());
        activity?.SetTag("report.id", report.Id.ToString());

        logger.LogInformation("Period {PeriodId} closed, new period {NewPeriodId} started, report {ReportId} generated",
            currentPeriod.Id, newPeriod.Id, report.Id);

        return new ClosePeriodResponse(currentPeriod.Id, newPeriod.Id, report.Id);
    }
}
