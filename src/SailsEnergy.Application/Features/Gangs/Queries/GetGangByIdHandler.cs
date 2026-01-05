using Marten;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Features.Gangs.Queries;

public static class GetGangByIdHandler
{
    public static async Task<GangResponse?> HandleAsync(
        GetGangByIdQuery query,
        IDocumentSession session,
        CancellationToken ct)
    {
        var gang = await session.LoadAsync<Gang>(query.GangId, ct);

        return gang is null
            ? null
            : new GangResponse(
                gang.Id,
                gang.Name,
                gang.Description,
                gang.OwnerId,
                gang.CreatedAt);
    }
}
