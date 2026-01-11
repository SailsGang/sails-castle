using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Application.Abstractions;

/// <summary>
/// Service for gang-related authorization checks.
/// Reduces duplication of membership validation across handlers.
/// </summary>
public interface IGangAuthorizationService
{
    /// <summary>
    /// Requires the current user to be an active member of the gang.
    /// </summary>
    Task<GangMember> RequireMembershipAsync(Guid gangId, CancellationToken ct = default);

    /// <summary>
    /// Requires the current user to be an Owner or Admin of the gang.
    /// </summary>
    Task<GangMember> RequireAdminAsync(Guid gangId, CancellationToken ct = default);

    /// <summary>
    /// Requires the current user to be the Owner of the gang.
    /// </summary>
    Task<GangMember> RequireOwnerAsync(Guid gangId, CancellationToken ct = default);

    /// <summary>
    /// Checks if current user is a member without throwing.
    /// </summary>
    Task<bool> IsMemberAsync(Guid gangId, CancellationToken ct = default);

    /// <summary>
    /// Checks if current user has the specified role or higher.
    /// </summary>
    Task<bool> HasRoleAsync(Guid gangId, MemberRole minimumRole, CancellationToken ct = default);
}
