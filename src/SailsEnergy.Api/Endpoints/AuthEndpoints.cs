using SailsEnergy.Api.Filters;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Application.Common;
using SailsEnergy.Application.Features.Auth.Commands.Login;
using SailsEnergy.Application.Features.Auth.Commands.Logout;
using SailsEnergy.Application.Features.Auth.Commands.RefreshToken;
using SailsEnergy.Application.Features.Auth.Commands.Register;
using SailsEnergy.Application.Features.Auth.Responses;
using Wolverine;

namespace SailsEnergy.Api.Endpoints;

internal static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/register", async (
                RegisterCommand command,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<AuthResult>(command, ct);

                return result.IsFailure
                    ? Results.Problem(
                        detail: result.ErrorMessage,
                        title: result.ErrorCode,
                        statusCode: StatusCodes.Status422UnprocessableEntity)
                    : Results.Created($"/api/users/{result.UserId}",
                        new AuthSuccessResponse(
                            result.AccessToken!,
                            result.RefreshToken!,
                            result.ExpiresAt!.Value,
                            result.UserId!.Value,
                            result.Email!,
                            result.DisplayName!));
            })
            .AddEndpointFilter<ValidationFilter<RegisterCommand>>()
            .RequireRateLimiting("auth")
            .WithName("Register")
            .WithDescription("Creates a new user account and returns JWT tokens.")
            .AllowAnonymous();

        group.MapPost("/login", async (
                LoginCommand command,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<AuthResult>(command, ct);

                return result.IsFailure
                    ? Results.Problem(
                        detail: result.ErrorMessage,
                        title: result.ErrorCode,
                        statusCode: StatusCodes.Status422UnprocessableEntity)
                    : Results.Ok(new AuthSuccessResponse(
                        result.AccessToken!,
                        result.RefreshToken!,
                        result.ExpiresAt!.Value,
                        result.UserId!.Value,
                        result.Email!,
                        result.DisplayName!));
            })
            .AddEndpointFilter<ValidationFilter<LoginCommand>>()
            .RequireRateLimiting("auth")
            .WithName("Login")
            .WithDescription("Authenticates user and returns JWT tokens.")
            .AllowAnonymous();

        group.MapPost("/refresh", async (
                RefreshTokenCommand command,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<AuthResult>(command, ct);

                return result.IsFailure
                    ? Results.Problem(
                        detail: result.ErrorMessage,
                        title: result.ErrorCode,
                        statusCode: StatusCodes.Status422UnprocessableEntity)
                    : Results.Ok(new AuthSuccessResponse(
                        result.AccessToken!,
                        result.RefreshToken!,
                        result.ExpiresAt!.Value,
                        result.UserId!.Value,
                        result.Email!,
                        result.DisplayName!));
            })
            .AddEndpointFilter<ValidationFilter<RefreshTokenCommand>>()
            .WithName("RefreshToken")
            .WithDescription("Exchanges a valid refresh token for new JWT tokens.")
            .AllowAnonymous();

        group.MapPost("/logout", async (
                IMessageBus bus,
                CancellationToken ct) =>
            {
                await bus.InvokeAsync(new LogoutCommand(), ct);
                return Results.NoContent();
            })
            .WithName("Logout")
            .WithDescription("Invalidates the current user's refresh token.")
            .RequireAuthorization();
    }
}
