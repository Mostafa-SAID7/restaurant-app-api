using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Auth.Models;

namespace RestaurantAPI.Data.Seeds;

/// <summary>
/// Seeder for authentication entities: roles and default admin user.
/// Creates default roles: Customer, RestaurantOwner, Admin.
/// </summary>
public class AuthSeeder : IDataSeeder
{
    private readonly ILogger<AuthSeeder> _logger;

    public AuthSeeder(ILogger<AuthSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(AppDbContext context)
    {
        _logger.LogInformation("Seeding authentication data...");

        await SeedRolesAsync(context);

        _logger.LogInformation("Authentication data seeded successfully");
    }

    /// <summary>
    /// Seeds default roles into database if they don't exist.
    /// </summary>
    private async Task SeedRolesAsync(AppDbContext context)
    {
        var roleCount = await context.Roles.CountAsync();
        if (roleCount > 0)
        {
            _logger.LogInformation("Roles already exist, skipping seed");
            return;
        }

        var roles = new List<ApplicationRole>
        {
            new()
            {
                Name = "Customer",
                Description = "Standard user role for customers",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "RestaurantOwner",
                Description = "Restaurant owner role with menu management permissions",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Admin",
                Description = "Administrator role with full system access",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        _logger.LogInformation("Seeded {RoleCount} default roles", roles.Count);
    }
}
