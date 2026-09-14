using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for Cart entity.
/// </summary>
public static class CartConfiguration
{
    /// <summary>
    /// Applies Cart entity configuration to the model builder.
    /// </summary>
    public static void ConfigureCart(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.CartID);
            
            entity.Property(c => c.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Quantity).IsRequired();
            entity.Property(c => c.ItemPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();

            // Foreign key: Cart belongs to User (cascade delete)
            entity.HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key: Cart contains an Item (cascade delete)
            entity.HasOne(c => c.Item)
                .WithMany(i => i.Carts)
                .HasForeignKey(c => c.ItemID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
