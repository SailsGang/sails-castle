namespace SailsEnergy.Application.Features.Tariffs.Queries.GetTariffHistory;

public sealed record GetTariffHistoryQuery(Guid GangId, int Page = 1, int PageSize = 20);
