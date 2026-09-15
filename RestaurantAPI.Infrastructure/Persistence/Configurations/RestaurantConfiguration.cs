using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Infrastructure.Persistence.Configurations;

public static class RestaurantConfiguration
{
    public static void ConfigureRestaurant(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(r => r.RestaurantID);

            entity.Property(r => r.RestaurantName).IsRequired().HasMaxLength(255);
            entity.Property(r => r.Address).IsRequired().HasMaxLength(500);
            entity.Property(r => r.Type).IsRequired().HasMaxLength(100);
            entity.Property(r => r.CreatedAt).IsRequired();

            entity.HasMany(r => r.Items)
                .WithOne(i => i.Restaurant)
                .HasForeignKey(i => i.RestaurantID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.MasterOrders)
                .WithOne(mo => mo.Restaurant)
                .HasForeignKey(mo => mo.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
