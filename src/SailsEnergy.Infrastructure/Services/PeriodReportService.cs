using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Periods.Documents;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Infrastructure.Data;

namespace SailsEnergy.Infrastructure.Services;

public class PeriodReportService(AppDbContext dbContext) : IPeriodReportService
{
    public async Task<PeriodReport> GenerateReportAsync(
        Guid gangId,
        Period period,
        decimal pricePerKwh,
        string currency,
        Guid generatedByUserId,
        CancellationToken ct = default)
    {
        var energyLogs = await dbContext.EnergyLogs
            .Where(e => e.GangId == gangId &&
                        e.PeriodId == period.Id &&
                        !e.IsDeleted)
            .ToListAsync(ct);

        var memberUserIds = energyLogs.Select(e => e.LoggedByUserId).Distinct().ToList();
        var userProfiles = await dbContext.UserProfiles
            .Where(u => memberUserIds.Contains(u.IdentityId))
            .ToDictionaryAsync(u => u.IdentityId, u => u.DisplayName, ct);

        var carIds = energyLogs.Select(e => e.GangCarId).Distinct().ToList();
        var cars = await dbContext.Cars
            .Where(c => carIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name ?? $"{c.Manufacturer} {c.Model}", ct);

        return new PeriodReport
        {
            Id = Guid.NewGuid(),
            GangId = gangId,
            PeriodId = period.Id,
            PeriodStartedAt = period.StartedAt,
            PeriodClosedAt = DateTimeOffset.UtcNow,
            GeneratedAt = DateTimeOffset.UtcNow,
            GeneratedByUserId = generatedByUserId,
            TotalEnergyKwh = energyLogs.Sum(e => e.EnergyKwh),
            TotalCost = energyLogs.Sum(e => e.EnergyKwh * pricePerKwh),
            Currency = currency,
            TotalLogs = energyLogs.Count,
            MemberBreakdown = energyLogs
                .GroupBy(e => e.LoggedByUserId)
                .Select(g => new MemberSummary
                {
                    UserId = g.Key,
                    DisplayName = userProfiles.GetValueOrDefault(g.Key, "Unknown"),
                    EnergyKwh = g.Sum(e => e.EnergyKwh),
                    Cost = g.Sum(e => e.EnergyKwh * pricePerKwh),
                    LogCount = g.Count()
                })
                .ToList(),
            CarBreakdown = energyLogs
                .GroupBy(e => e.GangCarId)
                .Select(g => new CarSummary
                {
                    CarId = g.Key,
                    CarName = cars.GetValueOrDefault(g.Key, "Unknown"),
                    EnergyKwh = g.Sum(e => e.EnergyKwh),
                    Cost = g.Sum(e => e.EnergyKwh * pricePerKwh),
                    LogCount = g.Count()
                })
                .ToList()
        };
    }
}
