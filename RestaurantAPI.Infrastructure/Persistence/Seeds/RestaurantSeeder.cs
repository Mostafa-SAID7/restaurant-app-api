using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Seeds;

/// <summary>
/// Seeder for Restaurant entities.
/// Creates sample restaurants for development/testing.
/// </summary>
public class RestaurantSeeder : IDataSeeder
{
    private readonly ILogger<RestaurantSeeder> _logger;

    public RestaurantSeeder(ILogger<RestaurantSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(AppDbContext context)
    {
        _logger.LogInformation("Seeding restaurant data...");

        await SeedRestaurantsAsync(context);

        _logger.LogInformation("Restaurant data seeded successfully");
    }

    /// <summary>
    /// Seeds sample restaurants into database if none exist.
    /// </summary>
    private async Task SeedRestaurantsAsync(AppDbContext context)
    {
        var restaurantCount = await context.Restaurants.CountAsync();
        if (restaurantCount > 0)
        {
            _logger.LogInformation("Restaurants already exist, skipping seed");
            return;
        }

        var restaurants = new List<Restaurant>
        {
            new()
            {
                RestaurantName = "Pizza Palace",
                Address = "123 Main Street, Downtown",
                Type = "Italian",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                RestaurantName = "Sushi Haven",
                Address = "456 Oak Avenue, Midtown",
                Type = "Japanese",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                RestaurantName = "Burger Barn",
                Address = "789 Elm Road, Uptown",
                Type = "American",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Restaurants.AddRangeAsync(restaurants);
        await context.SaveChangesAsync();

        _logger.LogInformation("Seeded {RestaurantCount} sample restaurants", restaurants.Count);
    }
}
