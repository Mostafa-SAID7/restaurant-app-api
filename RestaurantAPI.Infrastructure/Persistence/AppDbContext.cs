using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Infrastructure.Persistence.Configurations;

namespace RestaurantAPI.Infrastructure.Persistence;

/// <summary>
/// Application database context (EF Core).
/// This is the ONLY place EF Core knowledge exists in the Infrastructure layer.
/// All entity configurations are delegated to configuration classes.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Authentication entities
    public DbSet<ApplicationRole> Roles { get; set; }
    public DbSet<ApplicationUserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // Business entities
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
