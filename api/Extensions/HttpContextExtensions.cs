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
    /// Gets the API key from query string
    /// </summary>
    public static string? GetApiKey(this HttpContext context)
    {
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