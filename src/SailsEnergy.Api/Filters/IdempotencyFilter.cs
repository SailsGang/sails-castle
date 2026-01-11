using SailsEnergy.Application.Abstractions;

namespace SailsEnergy.Api.Filters;

public class IdempotencyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var idempotencyKey = context.HttpContext.Request.Headers["X-Idempotency-Key"].FirstOrDefault();

        if (string.IsNullOrEmpty(idempotencyKey))
            return await next(context);

        var idempotencyService = context.HttpContext.RequestServices.GetRequiredService<IIdempotencyService>();

        var cachedResponse = await idempotencyService.GetCachedResponseAsync<object>(idempotencyKey);
        if (cachedResponse is not null)
        {
            context.HttpContext.Response.Headers.Append("X-Idempotency-Replayed", "true");
            return Results.Ok(cachedResponse);
        }

        if (await idempotencyService.IsProcessingAsync(idempotencyKey))
            return Results.Conflict(new { error = "REQUEST_IN_PROGRESS", message = "This request is currently being processed." });

        await idempotencyService.SetProcessingAsync(idempotencyKey, TimeSpan.FromSeconds(30));

        var result = await next(context);

        if (result is IValueHttpResult { Value: not null } valueResult)
            await idempotencyService.CacheResponseAsync(idempotencyKey, valueResult.Value);

        return result;
    }
}
