using System.Security.Claims;

namespace RestuarantAPI.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the user ID from the current HTTP context
    /// </summary>
    public static string? GetUserId(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets the user email from the current HTTP context
    /// </summary>
    public static string? GetUserEmail(this HttpContext context)
    {
        return context.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// <summary>
    /// Gets the API key from Authorization header (X-API-Key) or query string (legacy fallback)
    /// Prefers header-based authentication for security
    /// </summary>
    public static string? GetApiKey(this HttpContext context)
    {
        // Priority 1: X-API-Key header (recommended)
        var headerKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(headerKey))
        {
            return headerKey;
        }

        // Priority 2: Authorization header (Bearer token format)
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Priority 3: Query parameter (legacy fallback - not recommended for production)
        return context.Request.Query["apikey"].FirstOrDefault();
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
}