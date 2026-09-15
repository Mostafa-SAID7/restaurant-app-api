using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Infrastructure.Persistence.Configurations;

public static class CartConfiguration
{
    public static void ConfigureCart(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.CartID);

            entity.Property(c => c.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Quantity).IsRequired();
            entity.Property(c => c.ItemPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();

            entity.HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Item)
                .WithMany(i => i.Carts)
                .HasForeignKey(c => c.ItemID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
