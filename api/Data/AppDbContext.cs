using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Auth.Models;
using RestaurantAPI.Data.Configurations;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data;

/// <summary>
/// Application database context.
/// This context serves as the database gateway for all entities.
/// Entity configuration is delegated to separate configuration classes in Data/Configurations/
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ========== AUTHENTICATION ENTITIES ==========
    public DbSet<ApplicationRole> Roles { get; set; }
    public DbSet<ApplicationUserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // ========== BUSINESS ENTITIES ==========
    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<MasterOrder> MasterOrders { get; set; }
    public DbSet<Cart> Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure auth entities
        modelBuilder.ConfigureAuthEntities();

        // Configure business entities
        modelBuilder.ConfigureUser();
        modelBuilder.ConfigureRestaurant();
        modelBuilder.ConfigureItem();
        modelBuilder.ConfigureOrder();
        modelBuilder.ConfigureMasterOrder();
        modelBuilder.ConfigureCart();
    }
}





