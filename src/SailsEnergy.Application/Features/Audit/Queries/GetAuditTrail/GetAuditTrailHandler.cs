using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Audit.Responses;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Features.Audit.Queries.GetAuditTrail;

public static class GetAuditTrailHandler
{
    public static async Task<AuditTrailResponse> HandleAsync(
        GetAuditTrailQuery query,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        var userId = currentUser.UserId!.Value;

        var membership = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == query.GangId && m.UserId == userId && m.IsActive, ct);
        if (membership?.Role is not (MemberRole.Owner or MemberRole.Admin))
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Only admins can view audit trail.");

        var energyLogsQuery = dbContext.EnergyLogs
            .Where(l => l.GangId == query.GangId);

        if (query.From.HasValue)
            energyLogsQuery = energyLogsQuery.Where(l => l.CreatedAt >= query.From.Value);
        if (query.To.HasValue)
            energyLogsQuery = energyLogsQuery.Where(l => l.CreatedAt <= query.To.Value);

        var energyLogs = await energyLogsQuery
            .OrderByDescending(l => l.CreatedAt)
            .Take(100)
            .Select(l => new
            {
                l.Id,
                l.LoggedByUserId,
                l.CreatedAt,
                l.EnergyKwh
            })
            .ToListAsync(ct);

        var userIds = energyLogs.Select(l => l.LoggedByUserId).Distinct().ToList();
        var userProfiles = await dbContext.UserProfiles
            .Where(u => userIds.Contains(u.IdentityId))
            .ToDictionaryAsync(u => u.IdentityId, u => u.DisplayName ?? "Unknown", ct);

        var entries = energyLogs.Select(log => new AuditEntryResponse(log.Id, "EnergyLog", log.Id, "Created",
            log.LoggedByUserId, userProfiles.GetValueOrDefault(log.LoggedByUserId, "Unknown"), log.CreatedAt, null,
            new { EnergyKwh = log.EnergyKwh })).ToList();

        var totalCount = entries.Count;
        var pagedItems = entries
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return new AuditTrailResponse(pagedItems, totalCount, query.Page, query.PageSize);
    }
}
