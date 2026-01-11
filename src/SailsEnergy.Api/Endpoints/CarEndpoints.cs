using SailsEnergy.Api.Filters;
using SailsEnergy.Api.Requests;
using SailsEnergy.Application.Features.Cars.Commands.CreateCar;
using SailsEnergy.Application.Features.Cars.Commands.DeleteCar;
using SailsEnergy.Application.Features.Cars.Commands.UpdateCar;
using SailsEnergy.Application.Features.Cars.Queries.GetCarById;
using SailsEnergy.Application.Features.Cars.Queries.GetMyCars;
using SailsEnergy.Application.Features.Cars.Responses;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

public static class CarEndpoints
{
    public static void MapCarEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cars")
            .WithTags("Cars")
            .RequireAuthorization()
            .RequireRateLimiting("api")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // GET /api/cars - Get my cars
        group.MapGet("/", async (IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<IReadOnlyList<CarResponse>>(new GetMyCarsQuery(), ct);
            return Results.Ok(result);
        })
        .Produces<IReadOnlyList<CarResponse>>()
        .WithName("GetMyCars")
        .WithDescription("Returns all cars owned by the current user.");

        // POST /api/cars - Create car
        group.MapPost("/", async (CreateCarCommand command, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<CreateCarResponse>(command, ct);
            return Results.Created($"/api/cars/{result.CarId}", result);
        })
        .Produces<CreateCarResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AddEndpointFilter<ValidationFilter<CreateCarCommand>>()
        .AddEndpointFilter<IdempotencyFilter>()
        .WithName("CreateCar")
        .WithDescription("Creates a new car for the current user.");

        // GET /api/cars/{id} - Get car by ID
        group.MapGet("/{id:guid}", async (Guid id, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<CarResponse?>(new GetCarByIdQuery(id), ct);
            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .Produces<CarResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetCar")
        .WithDescription("Returns details of a specific car.");

        // PUT /api/cars/{id} - Update car
        group.MapPut("/{id:guid}", async (Guid id, UpdateCarRequest request, IMessageBus bus, CancellationToken ct) =>
        {
            var command = new UpdateCarCommand(id, request.Name, request.Model,
                request.Manufacturer, request.LicensePlate, request.BatteryCapacityKwh);
            await bus.InvokeAsync(command, ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AddEndpointFilter<ValidationFilter<UpdateCarCommand>>()
        .WithName("UpdateCar")
        .WithDescription("Updates a car. Only the owner can update.");

        // DELETE /api/cars/{id} - Delete car
        group.MapDelete("/{id:guid}", async (Guid id, IMessageBus bus, CancellationToken ct) =>
        {
            await bus.InvokeAsync(new DeleteCarCommand(id), ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteCar")
        .WithDescription("Soft deletes a car. Only the owner can delete.");
    }
}
