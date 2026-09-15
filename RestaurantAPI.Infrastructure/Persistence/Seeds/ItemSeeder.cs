using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Seeds;

/// <summary>
/// Seeder for Item (menu items) entities.
/// Creates sample menu items for each restaurant.
/// </summary>
public class ItemSeeder : IDataSeeder
{
    private readonly ILogger<ItemSeeder> _logger;

    public ItemSeeder(ILogger<ItemSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(AppDbContext context)
    {
        _logger.LogInformation("Seeding menu items data...");

        await SeedItemsAsync(context);

        _logger.LogInformation("Menu items data seeded successfully");
    }

    /// <summary>
    /// Seeds sample menu items into database if none exist.
    /// </summary>
    private async Task SeedItemsAsync(AppDbContext context)
    {
        var itemCount = await context.Items.CountAsync();
        if (itemCount > 0)
        {
            _logger.LogInformation("Menu items already exist, skipping seed");
            return;
        }

        // Get restaurants to associate items with
        var restaurants = await context.Restaurants.ToListAsync();
        if (!restaurants.Any())
        {
            _logger.LogWarning("No restaurants found, skipping items seed");
            return;
        }

        var items = new List<Item>();

        // Pizza items for first restaurant
        if (restaurants.Count > 0)
        {
            items.AddRange(new[]
            {
                new Item
                {
                    RestaurantID = restaurants[0].RestaurantID,
                    ItemName = "Margherita Pizza",
                    ItemDescription = "Classic pizza with tomato, mozzarella, and basil",
                    ItemPrice = 12.99m,
                    CreatedAt = DateTime.UtcNow
                },
                new Item
                {
                    RestaurantID = restaurants[0].RestaurantID,
                    ItemName = "Pepperoni Pizza",
                    ItemDescription = "Pizza topped with pepperoni and mozzarella cheese",
                    ItemPrice = 14.99m,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        // Sushi items for second restaurant
        if (restaurants.Count > 1)
        {
            items.AddRange(new[]
            {
                new Item
                {
                    RestaurantID = restaurants[1].RestaurantID,
                    ItemName = "California Roll",
                    ItemDescription = "Sushi roll with crab, avocado, and cucumber",
                    ItemPrice = 9.99m,
                    CreatedAt = DateTime.UtcNow
                },
                new Item
                {
                    RestaurantID = restaurants[1].RestaurantID,
                    ItemName = "Spicy Tuna Roll",
                    ItemDescription = "Spicy tuna sushi roll with sriracha",
                    ItemPrice = 10.99m,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        // Burger items for third restaurant
        if (restaurants.Count > 2)
        {
            items.AddRange(new[]
            {
                new Item
                {
                    RestaurantID = restaurants[2].RestaurantID,
                    ItemName = "Classic Burger",
                    ItemDescription = "Beef patty with lettuce, tomato, onion, and cheese",
                    ItemPrice = 11.99m,
                    CreatedAt = DateTime.UtcNow
                },
                new Item
                {
                    RestaurantID = restaurants[2].RestaurantID,
                    ItemName = "Bacon Cheeseburger",
                    ItemDescription = "Beef patty with bacon, cheese, and special sauce",
                    ItemPrice = 13.99m,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        await context.Items.AddRangeAsync(items);
        await context.SaveChangesAsync();

        _logger.LogInformation("Seeded {ItemCount} sample menu items", items.Count);
    }
}
