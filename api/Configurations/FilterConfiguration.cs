using RestaurantAPI.Filters;

namespace RestaurantAPI.Configurations;

public static class FilterConfiguration
{
    public static IServiceCollection AddFilterConfiguration(this IServiceCollection services)
    {
        // Register filters as services for dependency injection
        services.AddScoped<ApiKeyAuthorizationFilter>();
        services.AddScoped<GlobalExceptionFilter>();
        services.AddScoped<LoggingFilter>();
        services.AddScoped<RateLimitingFilter>();

        // Add memory cache for rate limiting
        services.AddMemoryCache();

        return services;
    }


}