using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Data.Seeds;

/// <summary>
/// Extension methods for database seeding during application startup.
/// </summary>
public static class DataSeedExtensions
{
    /// <summary>
    /// Seeds all data into the database using registered seeders.
    /// Should be called during application startup (e.g., in Program.cs).
    /// </summary>
    /// <param name="app">WebApplication instance</param>
    /// <returns>Task completion</returns>
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DataSeeding");

            try
            {
                logger.LogInformation("Starting database seeding...");

                // Apply migrations
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully");

                // Run seeders in order
                var seeders = new List<IDataSeeder>
                {
                    new AuthSeeder(loggerFactory.CreateLogger<AuthSeeder>()),
                    new RestaurantSeeder(loggerFactory.CreateLogger<RestaurantSeeder>()),
                    new ItemSeeder(loggerFactory.CreateLogger<ItemSeeder>())
                };

                foreach (var seeder in seeders)
                {
                    await seeder.SeedAsync(context);
                }

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during database seeding");
                throw;
            }
        }
    }

    /// <summary>
    /// Registers all database seeders in the dependency injection container.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddDataSeeders(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, AuthSeeder>();
        services.AddScoped<IDataSeeder, RestaurantSeeder>();
        services.AddScoped<IDataSeeder, ItemSeeder>();

        return services;
    }
}
