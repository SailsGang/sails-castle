using SailsEnergy.Api.Filters;
using SailsEnergy.Api.Requests;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Audit.Queries.GetAuditTrail;
using SailsEnergy.Application.Features.Audit.Responses;
using SailsEnergy.Application.Features.Gangs.Commands.AddCarToGang;
using SailsEnergy.Application.Features.Gangs.Commands.AddMember;
using SailsEnergy.Application.Features.Gangs.Commands.ChangeMemberRole;
using SailsEnergy.Application.Features.Gangs.Commands.CreateGang;
using SailsEnergy.Application.Features.Gangs.Commands.DeleteGang;
using SailsEnergy.Application.Features.Gangs.Commands.LeaveGang;
using SailsEnergy.Application.Features.Gangs.Commands.RemoveCarFromGang;
using SailsEnergy.Application.Features.Gangs.Commands.RemoveMember;
using SailsEnergy.Application.Features.Gangs.Commands.UpdateGang;
using SailsEnergy.Application.Features.Gangs.Queries.GetGang;
using SailsEnergy.Application.Features.Gangs.Queries.GetGangCars;
using SailsEnergy.Application.Features.Gangs.Queries.GetGangMembers;
using SailsEnergy.Application.Features.Gangs.Queries.GetMyGangs;
using SailsEnergy.Application.Features.Gangs.Responses;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

public static class GangEndpoints
{
    public static void MapGangEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/gangs")
            .WithTags("Gangs")
            .RequireAuthorization()
            .RequireRateLimiting("api")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // GET /api/gangs - Get my gangs
        group.MapGet("/", async (IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<IReadOnlyList<GangListItem>>(new GetMyGangsQuery(), ct);
            return Results.Ok(result);
        })
        .Produces<IReadOnlyList<GangListItem>>()
        .WithName("GetMyGangs")
        .WithDescription("Returns all gangs the current user is a member of, including their role in each gang.");

        // POST /api/gangs - Create gang
        group.MapPost("/", async (CreateGangCommand command, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<CreateGangResponse>(command, ct);
            return Results.Created($"/api/gangs/{result.GangId}", result);
        })
        .Produces<CreateGangResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AddEndpointFilter<ValidationFilter<CreateGangCommand>>()
        .AddEndpointFilter<IdempotencyFilter>()
        .WithName("CreateGang")
        .WithDescription("Creates a new gang with the current user as owner.");

        // GET /api/gangs/{id} - Get gang by ID
        group.MapGet("/{id:guid}", async (Guid id, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<GangResponse?>(new GetGangQuery(id), ct);
            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .Produces<GangResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetGang")
        .WithDescription("Returns details of a specific gang by ID.");

        // PUT /api/gangs/{id} - Update gang
        group.MapPut("/{id:guid}", async (Guid id, UpdateGangRequest request, IMessageBus bus, CancellationToken ct) =>
        {
            var command = new UpdateGangCommand(id, request.Name, request.Description);
            await bus.InvokeAsync(command, ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AddEndpointFilter<ValidationFilter<UpdateGangCommand>>()
        .WithName("UpdateGang")
        .WithDescription("Updates gang name and/or description. Only the owner can update.");

        // DELETE /api/gangs/{id} - Delete gang
        group.MapDelete("/{id:guid}", async (Guid id, IMessageBus bus, CancellationToken ct) =>
        {
            await bus.InvokeAsync(new DeleteGangCommand(id), ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteGang")
        .WithDescription("Soft deletes a gang. Only the owner can delete.");

        // GET /api/gangs/{id}/members - Get gang members
        group.MapGet("/{id:guid}/members", async (Guid id, int page, int pageSize, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<PaginatedResponse<GangMemberResponse>>(
                new GetGangMembersQuery(id, page > 0 ? page : 1, pageSize > 0 ? pageSize : 50), ct);
            return Results.Ok(result);
        })
        .Produces<PaginatedResponse<GangMemberResponse>>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetGangMembers")
        .WithDescription("Returns paginated active members of a gang.");

        // POST /api/gangs/{id}/members - Add member
        group.MapPost("/{id:guid}/members", async (
            Guid id,
            AddMemberRequest request,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            await bus.InvokeAsync(new AddMemberCommand(id, request.UserId), ct);
            return Results.Created($"/api/gangs/{id}/members", null);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .AddEndpointFilter<ValidationFilter<AddMemberCommand>>()
        .AddEndpointFilter<IdempotencyFilter>()
        .WithName("AddMember")
        .WithDescription("Adds a user as a member to the gang. Requires Owner or Admin role.");

        // DELETE /api/gangs/{id}/members/{memberId} - Remove member
        group.MapDelete("/{id:guid}/members/{memberId:guid}", async (
            Guid id,
            Guid memberId,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            await bus.InvokeAsync(new RemoveMemberCommand(id, memberId), ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("RemoveMember")
        .WithDescription("Removes a member from the gang. Owner, Admin, or self can remove.");

        // PUT /api/gangs/{id}/members/{memberId}/role - Change member role
        group.MapPut("/{id:guid}/members/{memberId:guid}/role", async (
            Guid id,
            Guid memberId,
            ChangeMemberRoleRequest request,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            await bus.InvokeAsync(new ChangeMemberRoleCommand(id, memberId, request.Role), ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AddEndpointFilter<ValidationFilter<ChangeMemberRoleCommand>>()
        .WithName("ChangeMemberRole")
        .WithDescription("Changes a member's role (Admin/Member). Only the owner can change roles.");

        // POST /api/gangs/{id}/leave - Leave gang (current user)
        group.MapPost("/{id:guid}/leave", async (Guid id, IMessageBus bus, CancellationToken ct) =>
        {
            await bus.InvokeAsync(new LeaveGangCommand(id), ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("LeaveGang")
        .WithDescription("Current user leaves the gang. Owner cannot leave without transferring ownership.");

        // GET /api/gangs/{id}/cars - Get gang cars
        group.MapGet("/{id:guid}/cars", async (Guid id, int page, int pageSize, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<PaginatedResponse<GangCarResponse>>(
                new GetGangCarsQuery(id, page > 0 ? page : 1, pageSize > 0 ? pageSize : 50), ct);
            return Results.Ok(result);
        })
        .Produces<PaginatedResponse<GangCarResponse>>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetGangCars")
        .WithDescription("Returns paginated active cars assigned to the gang.");

        // POST /api/gangs/{id}/cars - Add car to gang
        group.MapPost("/{id:guid}/cars", async (
            Guid id,
            AddCarToGangRequest request,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            await bus.InvokeAsync(new AddCarToGangCommand(id, request.CarId), ct);
            return Results.Created($"/api/gangs/{id}/cars", null);
        })
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .AddEndpointFilter<ValidationFilter<AddCarToGangCommand>>()
        .WithName("AddCarToGang")
        .WithDescription("Adds a car to the gang. Only the car owner can add their car.");

        // DELETE /api/gangs/{id}/cars/{carId} - Remove car from gang
        group.MapDelete("/{id:guid}/cars/{carId:guid}", async (
            Guid id,
            Guid carId,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            await bus.InvokeAsync(new RemoveCarFromGangCommand(id, carId), ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("RemoveCarFromGang")
        .WithDescription("Removes a car from the gang. Owner, Admin, or car owner can remove.");

        // GET /api/gangs/{id}/audit - Get audit trail
        group.MapGet("/{id:guid}/audit", async (
                Guid id,
                DateTimeOffset? from,
                DateTimeOffset? to,
                int page,
                int pageSize,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<AuditTrailResponse>(
                    new GetAuditTrailQuery(id, from, to, page > 0 ? page : 1, pageSize > 0 ? pageSize : 50), ct);
                return Results.Ok(result);
            })
            .WithTags("Audit")
            .RequireAuthorization()
            .Produces<AuditTrailResponse>()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .WithName("GetAuditTrail")
            .WithDescription("Returns audit trail for a gang. Admins only.");
    }
}
