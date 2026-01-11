using Microsoft.AspNetCore.Mvc;
using SailsEnergy.Api.Filters;
using SailsEnergy.Api.Requests;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Tariffs.Commands.SetTariff;
using SailsEnergy.Application.Features.Tariffs.Queries.GetCurrentTariff;
using SailsEnergy.Application.Features.Tariffs.Queries.GetTariffHistory;
using SailsEnergy.Application.Features.Tariffs.Responses;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

public static class TariffEndpoints
{
    public static void MapTariffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/gangs/{gangId:guid}/tariff")
            .WithTags("Tariffs")
            .RequireAuthorization();

        group.MapGet("", async (
            [FromRoute] Guid gangId,
            IMessageBus bus) =>
        {
            var result = await bus.InvokeAsync<TariffResponse?>(
                new GetCurrentTariffQuery(gangId));

            return result is null ? Results.NotFound(new { Message = "No tariff set for this gang." }) : Results.Ok(result);
        })
        .WithName("GetCurrentTariff")
        .WithSummary("Get current tariff for a gang")
        .Produces<TariffResponse>()
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("", async (
            [FromRoute] Guid gangId,
            [FromBody] SetTariffRequest request,
            IMessageBus bus) =>
        {
            var result = await bus.InvokeAsync<TariffResponse>(
                new SetTariffCommand(gangId, request.PricePerKwh, request.Currency));

            return Results.Created($"/api/gangs/{gangId}/tariff", result);
        })
        .WithName("SetTariff")
        .WithSummary("Set or update tariff price for a gang")
        .Produces<TariffResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesValidationProblem()
        .AddEndpointFilter<IdempotencyFilter>();

        group.MapGet("/history", async (
            [FromRoute] Guid gangId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IMessageBus bus) =>
        {
            var result = await bus.InvokeAsync<PaginatedResponse<TariffHistoryEntry>>(
                new GetTariffHistoryQuery(gangId, page > 0 ? page : 1, pageSize > 0 ? pageSize : 20));

            return Results.Ok(result);
        })
        .WithName("GetTariffHistory")
        .WithSummary("Get tariff change history for a gang")
        .Produces<PaginatedResponse<TariffHistoryEntry>>()
        .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
