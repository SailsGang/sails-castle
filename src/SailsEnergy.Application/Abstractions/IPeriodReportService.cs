using SailsEnergy.Application.Features.Periods.Documents;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Abstractions;

/// <summary>
/// Service for generating period reports.
/// </summary>
public interface IPeriodReportService
{
    /// <summary>
    /// Generates a period report with full breakdown of energy consumption.
    /// </summary>
    Task<PeriodReport> GenerateReportAsync(
        Guid gangId,
        Period period,
        decimal pricePerKwh,
        string currency,
        Guid generatedByUserId,
        CancellationToken ct = default);
}
