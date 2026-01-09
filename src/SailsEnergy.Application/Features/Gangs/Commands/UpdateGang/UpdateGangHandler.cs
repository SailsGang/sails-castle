using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Gangs.Responses;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Gangs.Commands.UpdateGang;

public static class UpdateGangHandler
{
    public static async Task HandleAsync(
        UpdateGangCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        CancellationToken ct)
    {
        var gang = await dbContext.Gangs.FindAsync([command.GangId], ct)
            ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Gang not found.");

        if (gang.OwnerId != currentUser.UserId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "Only owner can update the gang.");

        if (command.Name is not null)
            gang.SetName(command.Name, currentUser.UserId!.Value);
        if (command.Description is not null)
            gang.SetDescription(command.Description, currentUser.UserId!.Value);

        await dbContext.SaveChangesAsync(ct);
        await cache.InvalidateEntityAsync<GangResponse>(command.GangId, ct);
    }
}
