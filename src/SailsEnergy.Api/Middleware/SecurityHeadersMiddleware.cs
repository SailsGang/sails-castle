namespace SailsEnergy.Api.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Skip security headers for Scalar/OpenAPI endpoints in development
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var isApiDocsPath = path.StartsWith("/scalar") || path.StartsWith("/openapi");

        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Relaxed CSP for Scalar, strict for everything else
        if (!isApiDocsPath)
        {
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
        }

        if (context.Request.IsHttps)
            context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

        await next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}
