using Marten;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Entities;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands;

public static class CreateGangHandler
{
    public static async Task<CreateGangResponse> HandleAsync(
        CreateGangCommand command,
        IDocumentSession session,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new BusinessRuleException(ErrorCodes.Unauthorized, "Authentication required.");

        var userId = currentUserService.UserId!.Value;

        var gang = Gang.Create(command.Name, userId, command.Description);

        session.Store(gang);
        await session.SaveChangesAsync(cancellationToken);

        return new CreateGangResponse(gang.Id);
    }
}
