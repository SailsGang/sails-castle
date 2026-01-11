using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.EnergyLogs.Responses;

namespace SailsEnergy.Application.Features.EnergyLogs.Queries.GetMyEnergyLogs;

public static class GetMyEnergyLogsHandler
{
    private static readonly TimeSpan _editWindow = TimeSpan.FromMinutes(5);
    public static async Task<PaginatedResponse<EnergyLogResponse>> HandleAsync(
        GetMyEnergyLogsQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        var logsQuery = dbContext.EnergyLogs
            .Where(l => l.LoggedByUserId == userId);

        if (query.GangId.HasValue)
            logsQuery = logsQuery.Where(l => l.GangId == query.GangId.Value);

        if (query.PeriodId.HasValue)
            logsQuery = logsQuery.Where(l => l.PeriodId == query.PeriodId.Value);

        var totalCount = await logsQuery.CountAsync(ct);

        var logs = await logsQuery
            .OrderByDescending(l => l.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        if (logs.Count == 0)
            return new PaginatedResponse<EnergyLogResponse>([], totalCount, query.Page, query.PageSize);

        var gangCarIds = logs.Select(l => l.GangCarId).Distinct().ToList();
        var tariffIds = logs.Select(l => l.AppliedTariffId).Distinct().ToList();

        var carLookup = await dbContext.GangCars
            .Where(gc => gangCarIds.Contains(gc.Id))
            .Join(dbContext.Cars, gc => gc.CarId, c => c.Id, (gc, c) => new { gc.Id, c.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name ?? "Unknown", ct);

        var tariffLookup = await dbContext.Tariffs
            .Where(t => tariffIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.PricePerKwh, ct);

        var userProfile = await dbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.IdentityId == userId, ct);

        var now = DateTimeOffset.UtcNow;

        var items = logs.Select(l => new EnergyLogResponse(
            l.Id,
            l.GangId,
            l.GangCarId,
            carLookup.GetValueOrDefault(l.GangCarId, "Unknown"),
            l.PeriodId,
            l.LoggedByUserId,
            userProfile?.DisplayName ?? "Unknown",
            l.EnergyKwh,
            tariffLookup.TryGetValue(l.AppliedTariffId, out var price) ? l.EnergyKwh * price : 0,
            l.ChargingDate,
            l.Notes,
            now - l.CreatedAt <= _editWindow,
            l.CreatedAt
        )).ToList();

        return new PaginatedResponse<EnergyLogResponse>(items, totalCount, query.Page, query.PageSize);
    }
}
