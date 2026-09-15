using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Infrastructure.Persistence.Configurations;

public static class AuthConfiguration
{
    public static void ConfigureAuthEntities(this ModelBuilder modelBuilder)
    {
        ConfigureApplicationRole(modelBuilder);
        ConfigureApplicationUserRole(modelBuilder);
        ConfigureRefreshToken(modelBuilder);
    }

    private static void ConfigureApplicationRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Description).HasMaxLength(500);
            entity.Property(r => r.CreatedAt).IsRequired();

            entity.HasIndex(r => r.Name).IsUnique();

            entity.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureApplicationUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUserRole>(entity =>
        {
            entity.HasKey(ur => ur.Id);

            entity.Property(ur => ur.UserId).IsRequired();
            entity.Property(ur => ur.RoleId).IsRequired();
            entity.Property(ur => ur.AssignedAt).IsRequired();

            entity.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .HasPrincipalKey(u => u.Usercode)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
        });
    }

    private static void ConfigureRefreshToken(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.UserId).IsRequired();
            entity.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(500);
            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();
            entity.Property(rt => rt.CreatedByIp).HasMaxLength(45);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .HasPrincipalKey(u => u.Usercode)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(rt => rt.UserId);
            entity.HasIndex(rt => rt.ExpiresAt);
        });
    }
}
