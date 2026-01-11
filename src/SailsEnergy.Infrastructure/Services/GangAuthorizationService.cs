using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Infrastructure.Services;

public class GangAuthorizationService(IAppDbContext dbContext, ICurrentUserService currentUser)
    : IGangAuthorizationService
{
    public async Task<GangMember> RequireMembershipAsync(Guid gangId, CancellationToken ct = default)
    {
        var userId = currentUser.UserId
            ?? throw new BusinessRuleException(ErrorCodes.Unauthorized, "User not authenticated.");

        var membership = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == gangId && m.UserId == userId && m.IsActive, ct);

        return membership ?? throw new BusinessRuleException(ErrorCodes.Forbidden, "You are not a member of this gang.");
    }

    public async Task<GangMember> RequireAdminAsync(Guid gangId, CancellationToken ct = default)
    {
        var membership = await RequireMembershipAsync(gangId, ct);

        return membership.Role is not (MemberRole.Owner or MemberRole.Admin) ? throw new BusinessRuleException(ErrorCodes.Forbidden, "Only Owner or Admin can perform this action.") : membership;
    }

    public async Task<GangMember> RequireOwnerAsync(Guid gangId, CancellationToken ct = default)
    {
        var membership = await RequireMembershipAsync(gangId, ct);

        return membership.Role != MemberRole.Owner ? throw new BusinessRuleException(ErrorCodes.Forbidden, "Only Owner can perform this action.") : membership;
    }

    public async Task<bool> IsMemberAsync(Guid gangId, CancellationToken ct = default)
    {
        if (!currentUser.UserId.HasValue)
            return false;

        return await dbContext.GangMembers
            .AnyAsync(m => m.GangId == gangId && m.UserId == currentUser.UserId && m.IsActive, ct);
    }

    public async Task<bool> HasRoleAsync(Guid gangId, MemberRole minimumRole, CancellationToken ct = default)
    {
        if (!currentUser.UserId.HasValue)
            return false;

        var membership = await dbContext.GangMembers
            .FirstOrDefaultAsync(m => m.GangId == gangId && m.UserId == currentUser.UserId && m.IsActive, ct);

        if (membership is null)
            return false;

        return minimumRole switch
        {
            MemberRole.Owner => membership.Role == MemberRole.Owner,
            MemberRole.Admin => membership.Role is MemberRole.Owner or MemberRole.Admin,
            _ => true
        };
    }
}
