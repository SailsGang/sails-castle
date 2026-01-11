using SailsEnergy.Api.Filters;
using SailsEnergy.Api.Requests;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.EnergyLogs.Commands.DeleteEnergyLog;
using SailsEnergy.Application.Features.EnergyLogs.Commands.LogEnergy;
using SailsEnergy.Application.Features.EnergyLogs.Commands.UpdateEnergyLog;
using SailsEnergy.Application.Features.EnergyLogs.Queries.GetEnergyLogById;
using SailsEnergy.Application.Features.EnergyLogs.Queries.GetGangEnergyLogs;
using SailsEnergy.Application.Features.EnergyLogs.Queries.GetMyEnergyLogs;
using SailsEnergy.Application.Features.EnergyLogs.Responses;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

public static class EnergyEndpoints
{
    public static void MapEnergyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/energy")
            .WithTags("Energy Logs")
            .RequireAuthorization()
            .RequireRateLimiting("energy")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // POST /api/energy - Log energy
        group.MapPost("/", async (LogEnergyCommand command, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<LogEnergyResponse>(command, ct);

            return Results.Created($"/api/energy/{result.LogId}", result);
        })
        .Produces<LogEnergyResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AddEndpointFilter<ValidationFilter<LogEnergyCommand>>()
        .AddEndpointFilter<IdempotencyFilter>()
        .WithName("LogEnergy")
        .WithDescription("Log energy consumption for a car in a gang.");

        // GET /api/energy - Get my logs (with pagination)
        group.MapGet("/", async (Guid? gangId, Guid? periodId, int page, int pageSize, IMessageBus bus, CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<PaginatedResponse<EnergyLogResponse>>(
                    new GetMyEnergyLogsQuery(gangId, periodId, page > 0 ? page : 1, pageSize > 0 ? pageSize : 50), ct);
                return Results.Ok(result);
            })
            .Produces<PaginatedResponse<EnergyLogResponse>>()
            .WithName("GetMyEnergyLogs")
            .WithDescription("Returns paginated energy logs created by the current user.");

        // GET /api/energy/{id} - Get log by ID
        group.MapGet("/{id:guid}", async (Guid id, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<EnergyLogResponse>(new GetEnergyLogByIdQuery(id), ct);

            return Results.Ok(result);
        })
        .Produces<EnergyLogResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetEnergyLog")
        .WithDescription("Returns details of a specific energy log.");

        // PUT /api/energy/{id} - Update log
        group.MapPut("/{id:guid}", async (Guid id, UpdateEnergyLogRequest request, IMessageBus bus, CancellationToken ct) =>
        {
            var command = new UpdateEnergyLogCommand(id, request.EnergyKwh, request.ChargingDate, request.Notes);

            await bus.InvokeAsync(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("UpdateEnergyLog")
        .WithDescription("Updates an energy log. Only the owner can update within 5 minutes.");

        // DELETE /api/energy/{id} - Delete log
        group.MapDelete("/{id:guid}", async (Guid id, IMessageBus bus, CancellationToken ct) =>
        {
            await bus.InvokeAsync(new DeleteEnergyLogCommand(id), ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteEnergyLog")
        .WithDescription("Deletes an energy log. Admins can delete anytime, owners within 5 minutes.");

        // GET /api/gangs/{gangId}/energy - Get gang logs (with pagination)
        app.MapGet("/api/gangs/{gangId:guid}/energy", async (Guid gangId, Guid? periodId, int page, int pageSize, IMessageBus bus, CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<PaginatedResponse<EnergyLogResponse>>(
                    new GetGangEnergyLogsQuery(gangId, periodId, page > 0 ? page : 1, pageSize > 0 ? pageSize : 50), ct);
                return Results.Ok(result);
            })
        .WithTags("Gangs", "Energy Logs")
        .RequireAuthorization()
        .Produces<IReadOnlyList<EnergyLogResponse>>()
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithName("GetGangEnergyLogs")
        .WithDescription("Returns all energy logs for a gang in the current or specified period.");
    }
}
