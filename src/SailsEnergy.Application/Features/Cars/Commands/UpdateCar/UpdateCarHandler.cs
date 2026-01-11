using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Cars.Responses;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Application.Features.Cars.Commands.UpdateCar;

public static class UpdateCarHandler
{
    public static async Task HandleAsync(
        UpdateCarCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ICacheService cache,
        ILogger<UpdateCarCommand> logger,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Cars.StartActivity("UpdateCar");
        activity?.SetTag("car.id", command.CarId.ToString());
        activity?.SetTag("user.id", currentUser.UserId?.ToString());
        var car = await dbContext.Cars.FindAsync([command.CarId], ct)
                  ?? throw new BusinessRuleException(ErrorCodes.NotFound, "Car not found.");

        if (car.OwnerId != currentUser.UserId)
            throw new BusinessRuleException(ErrorCodes.Forbidden, "You can only update your own cars.");

        logger.LogInformation("User {UserId} updating car {CarId}", currentUser.UserId, command.CarId);

        if (command.Name is not null)
            car.SetName(command.Name, currentUser.UserId!.Value);

        if (command.LicensePlate is not null)
            car.SetLicensePlate(command.LicensePlate, currentUser.UserId!.Value);

        await dbContext.SaveChangesAsync(ct);

        await cache.InvalidateEntityAsync<CarResponse>(command.CarId, ct);

        logger.LogInformation("Car {CarId} updated successfully", command.CarId);
    }
}
