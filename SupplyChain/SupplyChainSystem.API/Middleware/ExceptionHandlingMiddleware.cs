using System.Net;
using System.Text.Json;

namespace SupplyChainSystem.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled request failure.");
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                title = context.Response.StatusCode == 500 ? "An unexpected error occurred." : exception.Message,
                status = context.Response.StatusCode,
                traceId = context.TraceIdentifier
            }));
        }
    }
}
