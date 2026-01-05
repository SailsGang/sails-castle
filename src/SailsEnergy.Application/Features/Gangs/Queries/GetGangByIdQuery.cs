namespace SailsEnergy.Application.Features.Gangs.Queries;

public record GetGangByIdQuery(Guid GangId);

public record GangResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid OwnerId,
    DateTimeOffset CreatedAt);
