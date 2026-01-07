namespace SailsEnergy.Domain.Common;


public record AuditEvent(
    string EventType,
    string Category,
    Guid? ActorId,
    string? ActorEmail,
    Guid? ResourceId,
    string? ResourceType,
    bool Success,
    string? Details = null);
