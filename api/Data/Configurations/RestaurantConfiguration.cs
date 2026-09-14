using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for Restaurant entity.
/// </summary>
public static class RestaurantConfiguration
{
    /// <summary>
    /// Applies Restaurant entity configuration to the model builder.
    /// </summary>
    public static void ConfigureRestaurant(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(r => r.RestaurantID);
            
            entity.Property(r => r.RestaurantName).IsRequired().HasMaxLength(255);
            entity.Property(r => r.Address).IsRequired().HasMaxLength(500);
            entity.Property(r => r.Type).IsRequired().HasMaxLength(100);
            entity.Property(r => r.CreatedAt).IsRequired();

            // One restaurant has many items (cascade delete)
            entity.HasMany(r => r.Items)
                .WithOne(i => i.Restaurant)
                .HasForeignKey(i => i.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            // One restaurant has many master orders (restrict delete - preserve order history)
            entity.HasMany(r => r.MasterOrders)
                .WithOne(mo => mo.Restaurant)
                .HasForeignKey(mo => mo.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
