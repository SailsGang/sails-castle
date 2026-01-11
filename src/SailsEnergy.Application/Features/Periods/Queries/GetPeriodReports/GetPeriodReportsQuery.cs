namespace SailsEnergy.Application.Features.Periods.Queries.GetPeriodReports;

public sealed record GetPeriodReportsQuery(Guid GangId, int Page = 1, int PageSize = 10);
