using RestuarantAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace RestuarantAPI.Filters;

/// <summary>
/// Action filter to log request and response information
/// </summary>
public class LoggingFilter : IActionFilter
{
    private readonly ILogger<LoggingFilter> _logger;
    private const string StopwatchKey = "ActionStopwatch";

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = stopwatch;

        var request = context.HttpContext.Request;
        var clientIp = context.HttpContext.GetClientIpAddress();
        var userAgent = request.Headers["User-Agent"].ToString();
        var apiKey = context.HttpContext.GetApiKey();

        _logger.LogInformation(
            "Request started: {Method} {Path} from {ClientIp} | User-Agent: {UserAgent} | ApiKey: {ApiKey}",
            request.Method,
            request.Path,
            clientIp,
            userAgent,
            apiKey?.Truncate(8) + "..." ?? "None"
        );

        // Log request parameters if any
        if (context.ActionArguments.Any())
        {
            var parameters = string.Join(", ", 
                context.ActionArguments.Select(arg => $"{arg.Key}: {arg.Value?.GetType().Name}"));
            
            _logger.LogDebug("Request parameters: {Parameters}", parameters);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Items[StopwatchKey] is Stopwatch stopwatch)
        {
            stopwatch.Stop();
            
            var statusCode = context.HttpContext.Response.StatusCode;
            var duration = stopwatch.ElapsedMilliseconds;
            
            var logLevel = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
            
            _logger.Log(logLevel,
                "Request completed: {Method} {Path} responded {StatusCode} in {Duration}ms",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path,
                statusCode,
                duration
            );

            // Log slow requests
            if (duration > 1000) // More than 1 second
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {Duration}ms",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    duration
                );
            }
        }
    }
}

/// <summary>
/// Attribute to apply logging filter to controllers or actions
/// </summary>
public class LogRequestsAttribute : TypeFilterAttribute
{
    public LogRequestsAttribute() : base(typeof(LoggingFilter))
    {
    }
}