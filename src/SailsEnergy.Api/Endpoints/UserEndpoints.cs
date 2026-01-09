using SailsEnergy.Api.Filters;
using SailsEnergy.Application.Features.Users.Commands.UpdateProfile;
using SailsEnergy.Application.Features.Users.Queries.GetCurrentUser;
using SailsEnergy.Application.Features.Users.Responses;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/me")
            .WithTags("User Profile")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // GET /api/me - Get current user profile
        group.MapGet("/", async (IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<UserProfileResponse>(new GetCurrentUserQuery(), ct);
            return Results.Ok(result);
        })
        .Produces<UserProfileResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetCurrentUser")
        .WithDescription("Returns the current user's profile information.");

        // PUT /api/me - Update current user profile
        group.MapPut("/", async (UpdateProfileCommand command, IMessageBus bus, CancellationToken ct) =>
        {
            await bus.InvokeAsync(command, ct);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AddEndpointFilter<ValidationFilter<UpdateProfileCommand>>()
        .WithName("UpdateCurrentUser")
        .WithDescription("Updates the current user's profile. Only provided fields are updated.");
    }
}
