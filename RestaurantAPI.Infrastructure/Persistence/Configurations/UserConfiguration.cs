using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Infrastructure.Persistence.Configurations;

public static class UserConfiguration
{
    public static void ConfigureUser(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Usercode);

            entity.HasIndex(u => u.UserEmail).IsUnique();

            entity.Property(u => u.UserEmail).IsRequired().HasMaxLength(500);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();

            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.MasterOrders)
                .WithOne(mo => mo.User)
                .HasForeignKey(mo => mo.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
