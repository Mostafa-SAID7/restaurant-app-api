using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Infrastructure.Persistence.Configurations;

public static class MasterOrderConfiguration
{
    public static void ConfigureMasterOrder(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MasterOrder>(entity =>
        {
            entity.HasKey(mo => mo.MasterID);

            entity.Property(mo => mo.GrandTotal).HasPrecision(10, 2).IsRequired();
            entity.Property(mo => mo.CreatedAt).IsRequired();

            entity.HasOne(mo => mo.User)
                .WithMany(u => u.MasterOrders)
                .HasForeignKey(mo => mo.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mo => mo.Restaurant)
                .WithMany(r => r.MasterOrders)
                .HasForeignKey(mo => mo.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(mo => mo.Orders)
                .WithOne(o => o.MasterOrder)
                .HasForeignKey(o => o.MasterID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
