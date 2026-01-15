using SailsEnergy.Domain.Common;

namespace SailsEnergy.Domain.Entities;

/// <summary>
/// Audit log entry for sensitive operations
/// </summary>
public class AuditLog : Entity
{
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public Guid PerformedByUserId { get; private set; }
    public string? Details { get; private set; }
    public DateTimeOffset PerformedAt { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        string action,
        string entityType,
        Guid entityId,
        Guid performedByUserId,
        string? details = null)
    {
        return new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            PerformedByUserId = performedByUserId,
            Details = details,
            PerformedAt = DateTimeOffset.UtcNow
        };
    }
}

/// <summary>
/// Common audit action types
/// </summary>
public static class AuditActions
{
    public const string GangDeleted = "GANG_DELETED";
    public const string MemberRemoved = "MEMBER_REMOVED";
    public const string MemberRoleChanged = "MEMBER_ROLE_CHANGED";
    public const string OwnershipTransferred = "OWNERSHIP_TRANSFERRED";
    public const string CarRemoved = "CAR_REMOVED";
    public const string TariffChanged = "TARIFF_CHANGED";
    public const string PeriodClosed = "PERIOD_CLOSED";
}
