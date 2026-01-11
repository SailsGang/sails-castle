using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Features.Cars.Responses;
using SailsEnergy.Application.Telemetry;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Features.Cars.Commands.CreateCar;

public static class CreateCarHandler
{
    public static async Task<CreateCarResponse> HandleAsync(
        CreateCarCommand command,
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        ILogger<CreateCarCommand> logger,
        CancellationToken ct)
    {
        using var activity = ActivitySources.Cars.StartActivity("CreateCar");
        var userId = currentUser.UserId!.Value;
        activity?.SetTag("user.id", userId.ToString());

        logger.LogInformation("User {UserId} creating car '{Model}'", userId, command.Model);

        var car = Car.Create(userId, command.Model, command.Manufacturer, userId);
        if (command.Name is not null)
            car.SetName(command.Name, userId);
        if (command.LicensePlate is not null)
            car.SetLicensePlate(command.LicensePlate, userId);

        dbContext.Cars.Add(car);
        await dbContext.SaveChangesAsync(ct);

        activity?.SetTag("car.id", car.Id.ToString());
        logger.LogInformation("Car {CarId} created successfully", car.Id);

        return new CreateCarResponse(car.Id);
    }
}
