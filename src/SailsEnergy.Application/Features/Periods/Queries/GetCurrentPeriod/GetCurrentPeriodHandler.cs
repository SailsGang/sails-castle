using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Periods.Responses;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Periods.Queries.GetCurrentPeriod;

public static class GetCurrentPeriodHandler
{
    public static async Task<PeriodResponse> HandleAsync(
        GetCurrentPeriodQuery query,
        IAppDbContext dbContext,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Periods.StartActivity("GetCurrentPeriod");
        activity?.SetTag("gang.id", query.GangId.ToString());

        await gangAuth.RequireMembershipAsync(query.GangId, ct);

        var period = await cache.GetOrCreateAsync(
            CacheKeys.GangActivePeriod(query.GangId),
            async () =>
            {
                var p = await dbContext.Periods
                    .FirstOrDefaultAsync(x => x.GangId == query.GangId &&
                                             x.Status == PeriodStatus.Active, ct);
                return p;
            },
            TimeSpan.FromMinutes(10),
            ct);

        if (period is null)
            throw new BusinessRuleException(ErrorCodes.NotFound, "No active period found.");

        return new PeriodResponse(
            period.Id,
            period.GangId,
            period.Status.ToString(),
            period.StartedAt,
            period.ClosedAt,
            period.ClosedByUserId);
    }
}
