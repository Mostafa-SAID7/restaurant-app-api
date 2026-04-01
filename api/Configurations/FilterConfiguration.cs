using RestuarantAPI.Filters;

namespace RestuarantAPI.Configurations;

public static class FilterConfiguration
{
    public static IServiceCollection AddFilterConfiguration(this IServiceCollection services)
    {
        // Register filters as services for dependency injection
        services.AddScoped<ApiKeyAuthorizationFilter>();
        services.AddScoped<ValidationFilter>();
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
            // Add global exception filter
            options.Filters.Add<GlobalExceptionFilter>();
            
            // Add global validation filter
            options.Filters.Add<ValidationFilter>();
            
            // Add global logging filter (optional - can be applied selectively)
            // options.Filters.Add<LoggingFilter>();
        });

        return services;
    }
}