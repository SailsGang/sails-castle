using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Tariffs.Responses;
using SailsEnergy.Application.Telemetry;

namespace SailsEnergy.Application.Features.Tariffs.Queries.GetCurrentTariff;

public static class GetCurrentTariffHandler
{
    public static async Task<TariffResponse?> HandleAsync(
        GetCurrentTariffQuery query,
        IAppDbContext dbContext,
        IGangAuthorizationService gangAuth,
        ICacheService cache,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Tariffs.StartActivity("GetCurrentTariff");
        activity?.SetTag("gang.id", query.GangId.ToString());

        await gangAuth.RequireMembershipAsync(query.GangId, ct);

        var tariff = await cache.GetOrCreateAsync(
            CacheKeys.GangTariff(query.GangId),
            async () =>
            {
                return await dbContext.Tariffs
                    .Where(t => t.GangId == query.GangId && t.EffectiveFrom <= DateTimeOffset.UtcNow)
                    .OrderByDescending(t => t.EffectiveFrom)
                    .FirstOrDefaultAsync(ct);
            },
            TimeSpan.FromMinutes(15),
            ct);

        if (tariff is null)
            return null;

        return new TariffResponse(
            tariff.Id,
            tariff.GangId,
            tariff.PricePerKwh,
            tariff.Currency,
            tariff.EffectiveFrom,
            tariff.SetByUserId);
    }
}
