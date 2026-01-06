using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Api.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, "VALIDATION_ERROR", ve.Message),
            BusinessRuleException be => (StatusCodes.Status422UnprocessableEntity, be.Code, be.Message),
            DomainException de => (StatusCodes.Status400BadRequest, "DOMAIN_ERROR", de.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "Authentication required."),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception occurred");

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new { error = errorCode, message, timestamp = DateTimeOffset.UtcNow }, _jsonOptions),
            cancellationToken);

        return true;
    }
}
