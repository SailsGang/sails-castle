using Marten;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Periods.Documents;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Periods.Queries.GetReportById;

public static class GetReportByIdHandler
{
    public static async Task<PeriodReport> HandleAsync(
        GetReportByIdQuery query,
        IQuerySession querySession,
        IGangAuthorizationService gangAuth,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Periods.StartActivity("GetReportById");
        activity?.SetTag("gang.id", query.GangId.ToString());
        activity?.SetTag("report.id", query.ReportId.ToString());

        await gangAuth.RequireMembershipAsync(query.GangId, ct);

        var report = await querySession.Query<PeriodReport>()
            .FirstOrDefaultAsync(r => r.Id == query.ReportId && r.GangId == query.GangId, ct);

        return report ?? throw new BusinessRuleException("NOT_FOUND", "Report not found.");
    }
}
