using RestaurantAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        // Only configure SQL Server if not in test environment
        // Integration tests will override this in CustomWebApplicationFactory
        if (environment != "Test")
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    
                    sqlOptions.CommandTimeout(30);
                });

                // Enable sensitive data logging in development
                if (environment == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });
        }
        else
        {
            // For test environment, register a placeholder that will be replaced by integration tests
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("test-db");
            });
        }

        return services;
    }
}