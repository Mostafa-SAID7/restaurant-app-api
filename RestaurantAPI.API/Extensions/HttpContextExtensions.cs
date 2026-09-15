using System.Security.Claims;

namespace RestaurantAPI.API.Extensions;

/// <summary>
/// Extension methods for HttpContext — user identity, IP address, and request metadata helpers.
/// Used by middleware, filters, and controllers that need raw HttpContext access.
/// For business logic that needs the current user, prefer ICurrentUserService instead.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the user ID from the current HTTP context (JWT NameIdentifier claim).
    /// </summary>
    public static string? GetUserId(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets the user email from the current HTTP context (JWT Email claim).
    /// </summary>
    public static string? GetUserEmail(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets a unique identifier for the current request (JWT userId, IP, or "unknown").
    /// Used for rate limiting and logging.
    /// </summary>
    public static string GetRequestIdentifier(this HttpContext context)
    {
        var userId = context.GetUserId();
        if (!string.IsNullOrEmpty(userId))
            return $"user_{userId}";

        var clientIp = context.GetClientIpAddress();
        return !string.IsNullOrEmpty(clientIp) ? $"ip_{clientIp}" : "unknown";
    }

    /// <summary>
    /// Gets the client IP address.
    /// Checks X-Forwarded-For, X-Real-IP, then falls back to RemoteIpAddress.
    /// </summary>
    public static string? GetClientIpAddress(this HttpContext context)
    {
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
            return forwarded.Split(',')[0].Trim();

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// Checks if the request is from a mobile device (via User-Agent header).
    /// </summary>
    public static bool IsMobileRequest(this HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString().ToLower();
        var mobileKeywords = new[] { "mobile", "android", "iphone", "ipad", "tablet" };
        return mobileKeywords.Any(keyword => userAgent.Contains(keyword));
    }

    /// <summary>
    /// Gets the base URL of the current request (scheme + host).
    /// </summary>
    public static string GetBaseUrl(this HttpContext context)
    {
        var request = context.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}