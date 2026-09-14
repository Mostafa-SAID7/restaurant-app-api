using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for MasterOrder entity.
/// MasterOrder groups related orders from a single restaurant into one transaction.
/// </summary>
public static class MasterOrderConfiguration
{
    /// <summary>
    /// Applies MasterOrder entity configuration to the model builder.
    /// </summary>
    public static void ConfigureMasterOrder(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MasterOrder>(entity =>
        {
            entity.HasKey(mo => mo.MasterID);
            
            entity.Property(mo => mo.GrandTotal).HasPrecision(10, 2).IsRequired();
            entity.Property(mo => mo.CreatedAt).IsRequired();

            // Foreign key: MasterOrder belongs to User (cascade delete)
            entity.HasOne(mo => mo.User)
                .WithMany(u => u.MasterOrders)
                .HasForeignKey(mo => mo.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key: MasterOrder from a Restaurant (restrict delete - preserve order history)
            entity.HasOne(mo => mo.Restaurant)
                .WithMany(r => r.MasterOrders)
                .HasForeignKey(mo => mo.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            // One MasterOrder contains many individual Orders (cascade delete)
            entity.HasMany(mo => mo.Orders)
                .WithOne(o => o.MasterOrder)
                .HasForeignKey(o => o.MasterID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
