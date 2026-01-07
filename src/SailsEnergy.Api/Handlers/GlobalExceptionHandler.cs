using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Api.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
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

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = errorCode,
            Detail = message,
            Instance = context.Request.Path,
            Extensions =
            {
                ["timestamp"] = DateTimeOffset.UtcNow,
                ["correlationId"] = context.Response.Headers["X-Correlation-ID"].FirstOrDefault()
            }
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);


        return true;
    }
}
