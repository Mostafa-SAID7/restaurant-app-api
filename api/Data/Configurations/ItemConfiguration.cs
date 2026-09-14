using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for Item entity.
/// </summary>
public static class ItemConfiguration
{
    /// <summary>
    /// Applies Item entity configuration to the model builder.
    /// </summary>
    public static void ConfigureItem(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(i => i.ItemID);
            
            entity.Property(i => i.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(i => i.ItemDescription).HasMaxLength(500);
            entity.Property(i => i.ItemPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(i => i.ImageUrl).HasMaxLength(500);
            entity.Property(i => i.CreatedAt).IsRequired();

            // Foreign key to Restaurant (cascade delete when restaurant deleted)
            entity.HasOne(i => i.Restaurant)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            // One item has many cart entries (cascade delete)
            entity.HasMany(i => i.Carts)
                .WithOne(c => c.Item)
                .HasForeignKey(c => c.ItemID)
                .OnDelete(DeleteBehavior.Cascade);

            // One item has many orders (restrict delete - preserve order history)
            entity.HasMany(i => i.Orders)
                .WithOne(o => o.Item)
                .HasForeignKey(o => o.ItemID)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
