using SailsEnergy.Api.Filters;
using SailsEnergy.Application.Features.Auth.Commands;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

internal static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/register", async (RegisterCommand command, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<AuthResponse>(command);

                return Results.Created($"/api/users/{result.UserId}", result);
            })
            .AddEndpointFilter<ValidationFilter<RegisterCommand>>()
            .RequireRateLimiting("auth")
            .WithName("Register")
            .AllowAnonymous();

        group.MapPost("/login", async (LoginCommand command, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<AuthResponse>(command);

                return Results.Ok(result);
            })
            .AddEndpointFilter<ValidationFilter<LoginCommand>>()
            .RequireRateLimiting("auth")
            .WithName("Login")
            .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshTokenCommand command, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<AuthResponse>(command);
                return Results.Ok(result);
            })
            .AddEndpointFilter<ValidationFilter<RefreshTokenCommand>>()
            .WithName("RefreshToken")
            .AllowAnonymous();

        group.MapPost("/logout", async (IMessageBus bus) =>
            {
                await bus.InvokeAsync(new LogoutCommand());
                return Results.NoContent();
            })
            .WithName("Logout")
            .RequireAuthorization();
    }
}
