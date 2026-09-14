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

    public static IServiceCollection AddGlobalFilters(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // Add global exception filter only
            options.Filters.Add<GlobalExceptionFilter>();
            
            // Validation is handled by InvalidModelStateResponseFactory in ApiConfiguration
            // Other filters (LoggingFilter, RateLimitingFilter, ApiKeyAuthorizationFilter) are applied selectively via attributes
        });

        return services;
    }
}