using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Infrastructure.Persistence.Configurations;

public static class OrderConfiguration
{
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
    }
}
