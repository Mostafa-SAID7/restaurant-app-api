using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for User entity.
/// </summary>
public static class UserConfiguration
{
    /// <summary>
    /// Applies User entity configuration to the model builder.
    /// </summary>
    public static void ConfigureUser(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Usercode);
            
            entity.HasIndex(u => u.UserEmail).IsUnique();
            
            entity.Property(u => u.UserEmail).IsRequired().HasMaxLength(500);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();

            // Navigation: User has many orders
            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation: User has many cart items
            entity.HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Navigation: User has many master orders
            entity.HasMany(u => u.MasterOrders)
                .WithOne(mo => mo.User)
                .HasForeignKey(mo => mo.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
