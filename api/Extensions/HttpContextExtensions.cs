using System.Security.Claims;

namespace RestaurantAPI.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the user ID from the current HTTP context (JWT claim)
    /// </summary>
    public static string? GetUserId(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets the user email from the current HTTP context (JWT claim)
    /// </summary>
    public static string? GetUserEmail(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets a unique identifier for the current request (JWT userId, IP, or unknown)
    /// Used for rate limiting and logging
    /// </summary>
    public static string GetRequestIdentifier(this HttpContext context)
    {
        // Priority 1: JWT authenticated user ID
        var userId = context.GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user_{userId}";
        }

        // Priority 2: Client IP address
        var clientIp = context.GetClientIpAddress();
        return !string.IsNullOrEmpty(clientIp) ? $"ip_{clientIp}" : "unknown";
    }

    /// <summary>
    /// Gets the client IP address
    /// </summary>
    public static string? GetClientIpAddress(this HttpContext context)
    {
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
        }

        return ipAddress;
    }

    /// <summary>
    /// Checks if the request is from a mobile device
    /// </summary>
    public static bool IsMobileRequest(this HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString().ToLower();
        
        var mobileKeywords = new[] { "mobile", "android", "iphone", "ipad", "tablet" };
        
        return mobileKeywords.Any(keyword => userAgent.Contains(keyword));
    }

    /// <summary>
    /// Gets the base URL of the current request
    /// </summary>
    public static string GetBaseUrl(this HttpContext context)
    {
        var request = context.Request;
        return $"{request.Scheme}://{request.Host}";
    }

    /// <summary>
    /// DEPRECATED: Gets the API key from Authorization header or query string
    /// Use GetRequestIdentifier() instead for JWT-based identification
    /// This method is kept for backward compatibility only and should not be used
    /// </summary>
    [Obsolete("Use GetRequestIdentifier() instead for JWT-based identification", false)]
    public static string? GetApiKey(this HttpContext context)
    {
        // Priority 1: X-API-Key header (legacy API key - not recommended)
        var headerKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(headerKey))
        {
            return headerKey;
        }

        // Priority 2: Authorization header Bearer token (now JWT instead of API key)
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Priority 3: Query parameter (legacy fallback - REMOVED for security)
        // Query parameters appear in logs, browser history, referer headers
        return null;
    }
}