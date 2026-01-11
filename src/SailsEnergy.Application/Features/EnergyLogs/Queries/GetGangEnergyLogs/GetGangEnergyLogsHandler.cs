using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.EnergyLogs.Responses;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.EnergyLogs.Queries.GetGangEnergyLogs;

public static class GetGangEnergyLogsHandler
{
    private static readonly TimeSpan _editWindow = TimeSpan.FromMinutes(5);

    public static async Task<PaginatedResponse<EnergyLogResponse>> HandleAsync(
        GetGangEnergyLogsQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        var isMember = await dbContext.GangMembers
            .AnyAsync(m => m.GangId == query.GangId && m.UserId == userId && m.IsActive, ct);
        if (!isMember)
            throw new BusinessRuleException("NOT_MEMBER", "You are not a member of this gang.");

        Guid periodId;
        if (query.PeriodId.HasValue)
            periodId = query.PeriodId.Value;
        else
        {
            var activePeriod = await dbContext.Periods
                .FirstOrDefaultAsync(p => p.GangId == query.GangId && p.Status == PeriodStatus.Active, ct);

            if (activePeriod is null)
                return new PaginatedResponse<EnergyLogResponse>([], 0, query.Page, query.PageSize);

            periodId = activePeriod.Id;
        }

        var totalCount = await dbContext.EnergyLogs
            .CountAsync(l => l.GangId == query.GangId && l.PeriodId == periodId, ct);

        var logs = await dbContext.EnergyLogs
            .Where(l => l.GangId == query.GangId && l.PeriodId == periodId)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        if (logs.Count == 0)
            return new PaginatedResponse<EnergyLogResponse>([], totalCount, query.Page, query.PageSize);

        var gangCarIds = logs.Select(l => l.GangCarId).Distinct().ToList();
        var userIds = logs.Select(l => l.LoggedByUserId).Distinct().ToList();
        var tariffIds = logs.Select(l => l.AppliedTariffId).Distinct().ToList();

        var carLookup = await dbContext.GangCars
            .Where(gc => gangCarIds.Contains(gc.Id))
            .Join(dbContext.Cars, gc => gc.CarId, c => c.Id, (gc, c) => new { gc.Id, c.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name ?? "Unknown", ct);

        var userLookup = await dbContext.UserProfiles
            .Where(u => userIds.Contains(u.IdentityId))
            .ToDictionaryAsync(u => u.IdentityId, u => u.DisplayName ?? "Unknown", ct);

        var tariffLookup = await dbContext.Tariffs
            .Where(t => tariffIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.PricePerKwh, ct);

        var now = DateTimeOffset.UtcNow;

        var items = logs.Select(l => new EnergyLogResponse(
            l.Id,
            l.GangId,
            l.GangCarId,
            carLookup.GetValueOrDefault(l.GangCarId, "Unknown"),
            l.PeriodId,
            l.LoggedByUserId,
            userLookup.GetValueOrDefault(l.LoggedByUserId, "Unknown"),
            l.EnergyKwh,
            tariffLookup.TryGetValue(l.AppliedTariffId, out var price) ? l.EnergyKwh * price : 0,
            l.ChargingDate,
            l.Notes,
            l.LoggedByUserId == userId && now - l.CreatedAt <= _editWindow,
            l.CreatedAt
        )).ToList();

        return new PaginatedResponse<EnergyLogResponse>(items, totalCount, query.Page, query.PageSize);
    }
}
