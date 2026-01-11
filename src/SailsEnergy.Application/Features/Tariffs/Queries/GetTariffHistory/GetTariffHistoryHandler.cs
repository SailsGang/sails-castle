using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Tariffs.Responses;
using SailsEnergy.Application.Telemetry;

namespace SailsEnergy.Application.Features.Tariffs.Queries.GetTariffHistory;

public static class GetTariffHistoryHandler
{
    public static async Task<PaginatedResponse<TariffHistoryEntry>> HandleAsync(
        GetTariffHistoryQuery query,
        IAppDbContext dbContext,
        IGangAuthorizationService gangAuth,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Tariffs.StartActivity("GetTariffHistory");
        activity?.SetTag("gang.id", query.GangId.ToString());

        await gangAuth.RequireMembershipAsync(query.GangId, ct);

        var totalCount = await dbContext.Tariffs
            .CountAsync(t => t.GangId == query.GangId, ct);

        var tariffs = await dbContext.Tariffs
            .Where(t => t.GangId == query.GangId)
            .OrderByDescending(t => t.EffectiveFrom)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var userIds = tariffs.Select(t => t.SetByUserId).Distinct().ToList();
        var userProfiles = await dbContext.UserProfiles
            .Where(u => userIds.Contains(u.IdentityId))
            .ToDictionaryAsync(u => u.IdentityId, u => u.DisplayName, ct);

        var items = tariffs.Select(t => new TariffHistoryEntry(
            t.PricePerKwh,
            t.Currency,
            t.EffectiveFrom,
            t.SetByUserId,
            userProfiles.GetValueOrDefault(t.SetByUserId, "Unknown"))).ToList();

        return new PaginatedResponse<TariffHistoryEntry>(
            items,
            totalCount,
            query.Page,
            query.PageSize);
    }
}
