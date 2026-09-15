using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Infrastructure.Persistence.Configurations;

public static class ItemConfiguration
{
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

            entity.HasOne(i => i.Restaurant)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(i => i.Carts)
                .WithOne(c => c.Item)
                .HasForeignKey(c => c.ItemID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(i => i.Orders)
                .WithOne(o => o.Item)
                .HasForeignKey(o => o.ItemID)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
