namespace SailsEnergy.Application.Features.Gangs.Queries.GetGangCars;

public record GetGangCarsQuery(Guid GangId, int Page = 1, int PageSize = 50);
