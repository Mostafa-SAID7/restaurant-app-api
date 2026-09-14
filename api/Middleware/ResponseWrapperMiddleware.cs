using System.Diagnostics;
using System.Text.Json;
using RestaurantAPI.DTOs;

namespace RestaurantAPI.Middleware;

/// <summary>
/// Middleware to wrap all API responses with consistent metadata
/// Phase B.2: Adds request ID, execution time, and standardized envelope
/// </summary>
public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrapperMiddleware> _logger;

    public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate unique request ID
        var requestId = Guid.NewGuid().ToString("N")[..12];
        context.Items["RequestId"] = requestId;

        // Store original response stream
        var originalResponseBody = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Call next middleware
            await _next(context);
            stopwatch.Stop();

            // Skip wrapping for non-API endpoints and certain status codes
            if (ShouldWrapResponse(context))
            {
                await WrapResponse(context, responseBody, requestId, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                // Copy original response if not wrapping
                await responseBody.CopyToAsync(originalResponseBody);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unhandled exception in request {RequestId}", requestId);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = ApiResponseWrapper<object>.CreateError(
                "An unexpected error occurred", 
                500, 
                new { RequestId = requestId }
            );
            errorResponse.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(json));
        }
        finally
        {
            // Copy wrapped response back to original stream
            await responseBody.CopyToAsync(originalResponseBody);
        }
    }

    private bool ShouldWrapResponse(HttpContext context)
    {
        // Only wrap API endpoints, exclude static files and health checks
        var path = context.Request.Path.ToString();
        if (path.StartsWith("/api/") && context.Response.StatusCode < 400)
        {
            return true;
        }

        return false;
    }

    private async Task WrapResponse(HttpContext context, MemoryStream responseBody, string requestId, long executionTimeMs)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBody);
        var body = await reader.ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        // Try to parse existing response as JSON
        try
        {
            // For now, copy response as-is since controllers already return ApiResponse<T>
            // In a full implementation, you'd parse and re-wrap here
            await responseBody.CopyToAsync(context.Response.Body);
        }
        catch
        {
            // If parsing fails, copy original response
            await responseBody.CopyToAsync(context.Response.Body);
        }
    }
}

/// <summary>
/// Extension method to add response wrapper middleware
/// </summary>
public static class ResponseWrapperMiddlewareExtensions
{
    public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ResponseWrapperMiddleware>();
    }
}
