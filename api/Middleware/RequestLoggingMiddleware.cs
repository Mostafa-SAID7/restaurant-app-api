using System.Diagnostics;
using System.Text;

namespace RestaurantAPI.Middleware;

/// <summary>
/// Middleware for centralized request/response logging
/// Phase B.3: Logs all API interactions with structured data for auditing and debugging
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip logging for health checks and static files
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var requestId = context.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString("N")[..12];
        context.Items["RequestId"] = requestId;

        // Log incoming request
        await LogRequestAsync(context, requestId);

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

            // Log response
            await LogResponseAsync(context, responseBody, requestId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Request {RequestId} failed with exception after {ElapsedMs}ms", 
                requestId, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            // Copy response back to original stream
            await responseBody.CopyToAsync(originalResponseBody);
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;
        
        // Read request body if present
        string? requestBody = null;
        if (request.Method != "GET" && request.ContentLength > 0)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            request.Body.Position = 0; // Reset for actual processing
        }

        var logData = new
        {
            RequestId = requestId,
            Method = request.Method,
            Path = request.Path.ToString(),
            QueryString = request.QueryString.ToString(),
            Headers = GetSafeHeaders(request.Headers),
            ContentType = request.ContentType,
            ContentLength = request.ContentLength,
            RemoteIP = context.Connection.RemoteIpAddress?.ToString(),
            UserId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Body = requestBody,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation(
            "API Request: {Method} {Path} | RequestId: {RequestId} | User: {UserId}",
            request.Method,
            request.Path,
            requestId,
            logData.UserId ?? "Anonymous"
        );

        // Log full details at debug level
        _logger.LogDebug("Request details: {@RequestLog}", logData);
    }

    private async Task LogResponseAsync(HttpContext context, MemoryStream responseBody, string requestId, long elapsedMs)
    {
        var response = context.Response;
        
        // Read response body
        responseBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBody);
        var responseContent = await reader.ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        // Truncate large responses for logging
        var truncatedResponse = responseContent.Length > 2000 
            ? responseContent[..2000] + "... (truncated)" 
            : responseContent;

        var logData = new
        {
            RequestId = requestId,
            StatusCode = response.StatusCode,
            ContentType = response.ContentType,
            ContentLength = response.ContentLength,
            ExecutionTimeMs = elapsedMs,
            Body = truncatedResponse,
            Timestamp = DateTime.UtcNow
        };

        var logLevel = response.StatusCode >= 500 ? LogLevel.Error 
                      : response.StatusCode >= 400 ? LogLevel.Warning 
                      : LogLevel.Information;

        _logger.Log(
            logLevel,
            "API Response: {StatusCode} | RequestId: {RequestId} | Time: {ExecutionTimeMs}ms",
            response.StatusCode,
            requestId,
            elapsedMs
        );

        // Log full details at debug level
        _logger.LogDebug("Response details: {@ResponseLog}", logData);
    }

    private bool ShouldSkipLogging(PathString path)
    {
        var pathStr = path.ToString().ToLower();
        var skippedPaths = new[] { "/health", "/metrics", ".css", ".js", ".png", ".jpg", ".ico" };
        
        return skippedPaths.Any(p => pathStr.Contains(p));
    }

    private Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var safeHeaders = new Dictionary<string, string>();
        var sensitivHeaders = new[] { "authorization", "password", "token", "apikey", "secret" };

        foreach (var header in headers)
        {
            if (sensitivHeaders.Any(sh => header.Key.ToLower().Contains(sh)))
            {
                safeHeaders[header.Key] = "[REDACTED]";
            }
            else
            {
                safeHeaders[header.Key] = header.Value.ToString();
            }
        }

        return safeHeaders;
    }
}

/// <summary>
/// Extension method to add request logging middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
