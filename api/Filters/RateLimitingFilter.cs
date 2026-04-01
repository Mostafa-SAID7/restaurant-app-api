using FakeRestuarantAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace FakeRestuarantAPI.Filters;

/// <summary>
/// Action filter to implement basic rate limiting
/// </summary>
public class RateLimitingFilter : IActionFilter
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<RateLimitingFilter> _logger;
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;

    public RateLimitingFilter(IMemoryCache cache, ILogger<RateLimitingFilter> logger, int maxRequests = 100, int timeWindowMinutes = 1)
    {
        _cache = cache;
        _logger = logger;
        _maxRequests = maxRequests;
        _timeWindow = TimeSpan.FromMinutes(timeWindowMinutes);
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var clientIp = context.HttpContext.GetClientIpAddress() ?? "unknown";
        var apiKey = context.HttpContext.GetApiKey();
        
        // Use API key if available, otherwise use IP address
        var identifier = !apiKey.IsNullOrWhiteSpace() ? $"apikey_{apiKey}" : $"ip_{clientIp}";
        var cacheKey = $"rate_limit_{identifier}";

        var requestCount = _cache.Get<int?>(cacheKey) ?? 0;

        if (requestCount >= _maxRequests)
        {
            _logger.LogWarning(
                "Rate limit exceeded for {Identifier}. Requests: {RequestCount}/{MaxRequests}",
                identifier,
                requestCount,
                _maxRequests
            );

            context.Result = new ObjectResult(new
            {
                Success = false,
                Message = "Rate limit exceeded. Please try again later.",
                RetryAfter = _timeWindow.TotalSeconds,
                Timestamp = DateTime.UtcNow
            })
            {
                StatusCode = 429 // Too Many Requests
            };

            context.HttpContext.Response.Headers.Add("Retry-After", _timeWindow.TotalSeconds.ToString());
            return;
        }

        // Increment request count
        _cache.Set(cacheKey, requestCount + 1, _timeWindow);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}

/// <summary>
/// Attribute to apply rate limiting filter to controllers or actions
/// </summary>
public class RateLimitAttribute : TypeFilterAttribute
{
    public RateLimitAttribute(int maxRequests = 100, int timeWindowMinutes = 1) 
        : base(typeof(RateLimitingFilter))
    {
        Arguments = new object[] { maxRequests, timeWindowMinutes };
    }
}