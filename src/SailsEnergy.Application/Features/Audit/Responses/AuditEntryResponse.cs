namespace SailsEnergy.Application.Features.Audit.Responses;

public sealed record AuditEntryResponse(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Action,
    Guid PerformedByUserId,
    string PerformedByUserName,
    DateTimeOffset PerformedAt,
    object? OldValues,
    object? NewValues);

public sealed record AuditTrailResponse(
    IReadOnlyList<AuditEntryResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
