using Marten;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Periods.Documents;
using SailsEnergy.Application.Telemetry;

namespace SailsEnergy.Application.Features.Periods.Queries.GetPeriodReports;

public static class GetPeriodReportsHandler
{
    public static async Task<PaginatedResponse<PeriodReportSummary>> HandleAsync(
        GetPeriodReportsQuery query,
        IQuerySession querySession,
        IGangAuthorizationService gangAuth,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Periods.StartActivity("GetPeriodReports");
        activity?.SetTag("gang.id", query.GangId.ToString());

        await gangAuth.RequireMembershipAsync(query.GangId, ct);

        var reports = await querySession.Query<PeriodReport>()
            .Where(r => r.GangId == query.GangId)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync(ct);

        var totalCount = reports.Count;
        var pagedReports = reports
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new PeriodReportSummary(
                r.Id,
                r.PeriodId,
                r.PeriodStartedAt,
                r.PeriodClosedAt,
                r.TotalEnergyKwh,
                r.TotalCost,
                r.Currency,
                r.TotalLogs,
                r.GeneratedAt))
            .ToList();

        return new PaginatedResponse<PeriodReportSummary>(
            pagedReports,
            totalCount,
            query.Page,
            query.PageSize);
    }
}

public sealed record PeriodReportSummary(
    Guid ReportId,
    Guid PeriodId,
    DateTimeOffset PeriodStartedAt,
    DateTimeOffset PeriodClosedAt,
    decimal TotalEnergyKwh,
    decimal TotalCost,
    string Currency,
    int TotalLogs,
    DateTimeOffset GeneratedAt);
