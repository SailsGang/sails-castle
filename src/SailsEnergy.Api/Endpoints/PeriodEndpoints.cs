using Microsoft.AspNetCore.Mvc;
using SailsEnergy.Api.Filters;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Periods.Commands.ClosePeriod;
using SailsEnergy.Application.Features.Periods.Queries.GetCurrentPeriod;
using SailsEnergy.Application.Features.Periods.Queries.GetPeriodReports;
using SailsEnergy.Application.Features.Periods.Responses;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

public static class PeriodEndpoints
{
    public static void MapPeriodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/gangs/{gangId:guid}/period")
            .WithTags("Periods")
            .RequireAuthorization();

        group.MapGet("", async (
            [FromRoute] Guid gangId,
            IMessageBus bus) =>
        {
            var result = await bus.InvokeAsync<PeriodResponse>(
                new GetCurrentPeriodQuery(gangId));
            return Results.Ok(result);
        })
        .WithName("GetCurrentPeriod")
        .WithSummary("Get current active period for a gang")
        .Produces<PeriodResponse>()
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/close", async (
            [FromRoute] Guid gangId,
            IMessageBus bus) =>
        {
            var result = await bus.InvokeAsync<ClosePeriodResponse>(
                new ClosePeriodCommand(gangId));
            return Results.Ok(result);
        })
        .WithName("ClosePeriod")
        .WithSummary("Close current period, generate report, start new period")
        .Produces<ClosePeriodResponse>()
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status429TooManyRequests)
        .RequireRateLimiting("period-close")
        .AddEndpointFilter<IdempotencyFilter>();

        group.MapGet("/reports", async (
            [FromRoute] Guid gangId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IMessageBus bus) =>
        {
            var result = await bus.InvokeAsync<PaginatedResponse<PeriodReportSummary>>(
                new GetPeriodReportsQuery(gangId, page > 0 ? page : 1, pageSize > 0 ? pageSize : 10));
            return Results.Ok(result);
        })
        .WithName("GetPeriodReports")
        .WithSummary("Get historical period reports for a gang")
        .Produces<PaginatedResponse<PeriodReportSummary>>()
        .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
