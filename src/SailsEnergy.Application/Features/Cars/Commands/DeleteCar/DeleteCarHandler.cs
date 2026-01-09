using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Cars.Responses;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Cars.Commands.DeleteCar;

public static class DeleteCarHandler
{
    public static async Task HandleAsync(
        DeleteCarCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<DeleteCarCommand> logger,
        CancellationToken ct)
    {
        var car = await dbContext.Cars.FindAsync([command.CarId], ct)
                  ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Car not found.");

        if (car.OwnerId != currentUser.UserId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "You can only delete your own cars.");

        logger.LogInformation("User {UserId} deleting car {CarId}", currentUser.UserId, command.CarId);

        car.SoftDelete(currentUser.UserId!.Value);
        await dbContext.SaveChangesAsync(ct);

        await cache.InvalidateEntityAsync<CarResponse>(command.CarId, ct);

        logger.LogInformation("Car {CarId} deleted successfully", command.CarId);
    }
}
