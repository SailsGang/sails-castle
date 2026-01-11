using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.EnergyLogs.Responses;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.EnergyLogs.Queries.GetEnergyLogById;

public static class GetEnergyLogByIdHandler
{
    private static readonly TimeSpan _editWindow = TimeSpan.FromMinutes(5);
    public static async Task<EnergyLogResponse> HandleAsync(
        GetEnergyLogByIdQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        var log = await dbContext.EnergyLogs
            .FirstOrDefaultAsync(l => l.Id == query.LogId, ct);
        if (log is null)
            throw new BusinessRuleException("NOT_FOUND", "Energy log not found.");

        var isMember = await dbContext.GangMembers
            .AnyAsync(m => m.GangId == log.GangId && m.UserId == userId && m.IsActive, ct);
        if (!isMember)
            throw new BusinessRuleException("NOT_MEMBER", "You are not a member of this gang.");

        var car = await dbContext.GangCars
            .Where(gc => gc.Id == log.GangCarId)
            .Join(dbContext.Cars, gc => gc.CarId, c => c.Id, (gc, c) => c)
            .FirstOrDefaultAsync(ct);

        var user = await dbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.IdentityId == log.LoggedByUserId, ct);

        var tariff = await dbContext.Tariffs
            .FirstOrDefaultAsync(t => t.Id == log.AppliedTariffId, ct);

        var now = DateTimeOffset.UtcNow;

        return new EnergyLogResponse(
            log.Id,
            log.GangId,
            log.GangCarId,
            car?.Name ?? "Unknown",
            log.PeriodId,
            log.LoggedByUserId,
            user?.DisplayName ?? "Unknown",
            log.EnergyKwh,
            tariff != null ? log.EnergyKwh * tariff.PricePerKwh : 0,
            log.ChargingDate,
            log.Notes,
            log.LoggedByUserId == userId && now - log.CreatedAt <= _editWindow,
            log.CreatedAt
        );
    }
}
