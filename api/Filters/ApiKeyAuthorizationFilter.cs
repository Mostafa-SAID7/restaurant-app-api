using RestuarantAPI.Extensions;
using RestuarantAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestuarantAPI.Filters;

/// <summary>
/// Authorization filter to validate API keys for protected endpoints
/// </summary>
public class ApiKeyAuthorizationFilter : IAuthorizationFilter
{
    private readonly ILogger<ApiKeyAuthorizationFilter> _logger;

    public ApiKeyAuthorizationFilter(ILogger<ApiKeyAuthorizationFilter> logger)
    {
        _logger = logger;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.GetApiKey();

        if (apiKey.IsNullOrWhiteSpace())
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Success = false,
                Message = "API key is required",
                Timestamp = DateTime.UtcNow
            });
            return;
        }

        if (!ValidationHelper.IsValidApiKey(apiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Success = false,
                Message = "Invalid API key format",
                Timestamp = DateTime.UtcNow
            });
            return;
        }

        // Store the API key in HttpContext for use in controllers
        context.HttpContext.Items["ApiKey"] = apiKey;
    }
}

/// <summary>
/// Attribute to apply API key authorization to controllers or actions
/// </summary>
public class RequireApiKeyAttribute : TypeFilterAttribute
{
    public RequireApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter))
    {
    }
}