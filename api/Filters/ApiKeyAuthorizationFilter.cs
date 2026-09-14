using RestaurantAPI.Data;
using RestaurantAPI.Extensions;
using RestaurantAPI.Helpers;
using RestaurantAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Filters;

/// <summary>
/// Authorization filter to validate API keys (user codes) against the database
/// Supports X-API-Key header, Authorization Bearer token, or legacy query parameter
/// </summary>
public class ApiKeyAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly ILogger<ApiKeyAuthorizationFilter> _logger;
    private readonly IUserRepository _userRepository;

    public ApiKeyAuthorizationFilter(ILogger<ApiKeyAuthorizationFilter> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.GetApiKey();

        if (apiKey.IsNullOrWhiteSpace())
        {
            _logger.LogWarning("API request received without API key");
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "API key is required",
                timestamp = DateTime.UtcNow
            });
            return;
        }

        if (!ValidationHelper.IsValidApiKey(apiKey))
        {
            _logger.LogWarning($"API request received with invalid API key format: {apiKey}");
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "Invalid API key format",
                timestamp = DateTime.UtcNow
            });
            return;
        }

        // Validate that the API key belongs to an existing user
        var user = await _userRepository.GetByUserCodeAsync(apiKey);
        if (user == null)
        {
            _logger.LogWarning($"API request received with non-existent API key: {apiKey}");
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "Invalid API key - user not found",
                timestamp = DateTime.UtcNow
            });
            return;
        }

        // Store the API key and user in HttpContext for use in controllers
        context.HttpContext.Items["ApiKey"] = apiKey;
        context.HttpContext.Items["User"] = user;

        _logger.LogInformation($"API request authorized for user: {user.UserEmail}");
    }
}

/// <summary>
/// Attribute to require API key authorization on controllers or actions
/// </summary>
public class RequireApiKeyAttribute : TypeFilterAttribute
{
    public RequireApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter))
    {
    }
}
