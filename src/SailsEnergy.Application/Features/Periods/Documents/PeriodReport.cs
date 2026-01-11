namespace SailsEnergy.Application.Features.Periods.Documents;

/// <summary>
/// Immutable period report stored in Marten document store.
/// Generated when a period is closed.
/// </summary>
public class PeriodReport
{
    public Guid Id { get; set; }
    public Guid GangId { get; set; }
    public Guid PeriodId { get; set; }
    public DateTimeOffset PeriodStartedAt { get; set; }
    public DateTimeOffset PeriodClosedAt { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public Guid GeneratedByUserId { get; set; }

    // Totals
    public decimal TotalEnergyKwh { get; set; }
    public decimal TotalCost { get; set; }
    public string Currency { get; set; } = "UAH";
    public int TotalLogs { get; set; }

    // Breakdowns
    public List<MemberSummary> MemberBreakdown { get; set; } = [];
    public List<CarSummary> CarBreakdown { get; set; } = [];
}

public class MemberSummary
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public decimal EnergyKwh { get; set; }
    public decimal Cost { get; set; }
    public int LogCount { get; set; }
}

public class CarSummary
{
    public Guid CarId { get; set; }
    public string CarName { get; set; } = string.Empty;
    public decimal EnergyKwh { get; set; }
    public decimal Cost { get; set; }
    public int LogCount { get; set; }
}
