namespace SailsEnergy.Application.Features.Audit.Queries.GetAuditTrail;

public sealed record GetAuditTrailQuery(
    Guid GangId,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null,
    int Page = 1,
    int PageSize = 50);
