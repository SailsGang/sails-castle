namespace SailsEnergy.Application.Features.EnergyLogs.Queries.GetGangEnergyLogs;

public sealed record GetGangEnergyLogsQuery(
    Guid GangId,
    Guid? PeriodId = null,
    int Page = 1,
    int PageSize = 50);
