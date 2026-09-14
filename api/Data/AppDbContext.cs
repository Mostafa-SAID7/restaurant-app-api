using RestaurantAPI.Models;
using RestaurantAPI.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Data;

/// <summary>
/// Application database context with all entity configurations
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Business entities
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<MasterOrder> MasterOrders { get; set; }
    public DbSet<Cart> Carts { get; set; }

    // Auth entities
    public DbSet<ApplicationRole> Roles { get; set; }
    public DbSet<ApplicationUserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== AUTH ENTITIES ==========

        // ApplicationRole configuration
        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.HasKey(r => r.Id);
            
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Description).HasMaxLength(500);
            entity.Property(r => r.CreatedAt).IsRequired();

            // Unique constraint on role name
            entity.HasIndex(r => r.Name).IsUnique();

            // One role has many user-role mappings
            entity.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ApplicationUserRole configuration (junction table)
        modelBuilder.Entity<ApplicationUserRole>(entity =>
        {
            entity.HasKey(ur => ur.Id);

            entity.Property(ur => ur.UserId).IsRequired();
            entity.Property(ur => ur.RoleId).IsRequired();
            entity.Property(ur => ur.AssignedAt).IsRequired();

            // Many user-roles map to one user
            entity.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many user-roles map to one role
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite unique constraint: user cannot have same role twice
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.UserId).IsRequired();
            entity.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();
            entity.Property(rt => rt.CreatedByIp).HasMaxLength(45);
            entity.Property(rt => rt.RevokedAt);
            entity.Property(rt => rt.UsedAt);

            // Foreign key to User
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for looking up by user ID and checking expiration
            entity.HasIndex(rt => rt.UserId);
            entity.HasIndex(rt => rt.ExpiresAt);
        });

        // ========== BUSINESS ENTITIES ==========

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Usercode);
            
            entity.HasIndex(u => u.UserEmail).IsUnique();
            
            entity.Property(u => u.UserEmail).IsRequired().HasMaxLength(500);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
        });

        // Restaurant configuration
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(r => r.RestaurantID);
            
            entity.Property(r => r.RestaurantName).IsRequired().HasMaxLength(255);
            entity.Property(r => r.Address).IsRequired().HasMaxLength(500);
            entity.Property(r => r.Type).IsRequired().HasMaxLength(100);
            entity.Property(r => r.CreatedAt).IsRequired();

            // One restaurant has many items
            entity.HasMany(r => r.Items)
                .WithOne(i => i.Restaurant)
                .HasForeignKey(i => i.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            // One restaurant has many master orders
            entity.HasMany(r => r.MasterOrders)
                .WithOne(mo => mo.Restaurant)
                .HasForeignKey(mo => mo.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Item configuration
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(i => i.ItemID);
            
            entity.Property(i => i.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(i => i.ItemDescription).HasMaxLength(500);
            entity.Property(i => i.ItemPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(i => i.ImageUrl).HasMaxLength(500);
            entity.Property(i => i.CreatedAt).IsRequired();

            // Foreign key to Restaurant is already configured via navigation property
            entity.HasOne(i => i.Restaurant)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            // One item has many cart entries
            entity.HasMany(i => i.Carts)
                .WithOne(c => c.Item)
                .HasForeignKey(c => c.ItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // One item has many orders
            entity.HasMany(i => i.Orders)
                .WithOne(o => o.Item)
                .HasForeignKey(o => o.ItemID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User configuration - orders and carts
        modelBuilder.Entity<User>(entity =>
        {
            // One user has many orders
            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One user has many cart items
            entity.HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One user has many master orders
            entity.HasMany(u => u.MasterOrders)
                .WithOne(mo => mo.User)
                .HasForeignKey(mo => mo.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderID);
            
            entity.Property(o => o.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(o => o.Quantity).IsRequired();
            entity.Property(o => o.ItemPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(o => o.TotalPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(o => o.CreatedAt).IsRequired();

            // Foreign key relationships
            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.Item)
                .WithMany(i => i.Orders)
                .HasForeignKey(o => o.ItemID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.MasterOrder)
                .WithMany(mo => mo.Orders)
                .HasForeignKey(o => o.MasterID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Cart configuration
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.CartID);
            
            entity.Property(c => c.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Quantity).IsRequired();
            entity.Property(c => c.ItemPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();

            // Foreign key relationships
            entity.HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Item)
                .WithMany(i => i.Carts)
                .HasForeignKey(c => c.ItemID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MasterOrder configuration
        modelBuilder.Entity<MasterOrder>(entity =>
        {
            entity.HasKey(mo => mo.MasterID);
            
            entity.Property(mo => mo.GrandTotal).HasPrecision(10, 2).IsRequired();
            entity.Property(mo => mo.CreatedAt).IsRequired();

            // Foreign key relationships
            entity.HasOne(mo => mo.User)
                .WithMany(u => u.MasterOrders)
                .HasForeignKey(mo => mo.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mo => mo.Restaurant)
                .WithMany(r => r.MasterOrders)
                .HasForeignKey(mo => mo.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(mo => mo.Orders)
                .WithOne(o => o.MasterOrder)
                .HasForeignKey(o => o.MasterID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}





