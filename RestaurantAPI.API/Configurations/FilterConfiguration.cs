using RestaurantAPI.Filters;

namespace RestaurantAPI.Configurations;

public static class FilterConfiguration
{
    public static IServiceCollection AddFilterConfiguration(this IServiceCollection services)
    {
        // Register filters as services for dependency injection
        // ApiKeyAuthorizationFilter removed (migrated to JWT Bearer auth - Phase 2.8)
        services.AddScoped<GlobalExceptionFilter>();
        services.AddScoped<LoggingFilter>();
        services.AddScoped<RateLimitingFilter>();

        // Add memory cache for rate limiting
        services.AddMemoryCache();

        return services;
    }


}