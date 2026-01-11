namespace SailsEnergy.Application.Features.EnergyLogs.Queries.GetMyEnergyLogs;

public sealed record GetMyEnergyLogsQuery(
    Guid? GangId = null,
    Guid? PeriodId = null,
    int Page = 1,
    int PageSize = 50);
