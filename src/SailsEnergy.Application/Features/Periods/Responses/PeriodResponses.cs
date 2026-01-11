namespace SailsEnergy.Application.Features.Periods.Responses;

public sealed record PeriodResponse(
    Guid Id,
    Guid GangId,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? ClosedAt,
    Guid? ClosedByUserId);

public sealed record ClosePeriodResponse(
    Guid ClosedPeriodId,
    Guid NewPeriodId,
    Guid ReportId);
