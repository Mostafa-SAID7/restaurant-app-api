using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for Order entity.
/// </summary>
public static class OrderConfiguration
{
    /// <summary>
    /// Applies Order entity configuration to the model builder.
    /// </summary>
    public static void ConfigureOrder(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderID);
            
            entity.Property(o => o.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(o => o.Quantity).IsRequired();
            entity.Property(o => o.ItemPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(o => o.TotalPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(o => o.CreatedAt).IsRequired();

            // Foreign key: Order belongs to User (cascade delete - remove order if user deleted)
            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key: Order references Item (restrict delete - preserve order record if item deleted)
            entity.HasOne(o => o.Item)
                .WithMany(i => i.Orders)
                .HasForeignKey(o => o.ItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign key: Order belongs to MasterOrder (cascade delete - remove if master deleted)
            entity.HasOne(o => o.MasterOrder)
                .WithMany(mo => mo.Orders)
                .HasForeignKey(o => o.MasterID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
