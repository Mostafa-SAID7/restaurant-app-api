namespace RestaurantAPI.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration is now handled by Infrastructure.DependencyInjection.AddInfrastructure()
        // This method is kept for backward compatibility
        return services;
    }
}
