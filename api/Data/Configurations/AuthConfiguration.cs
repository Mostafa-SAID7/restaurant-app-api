using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Auth.Models;

namespace RestaurantAPI.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for authentication entities.
/// Configures: ApplicationRole, ApplicationUserRole, RefreshToken
/// </summary>
public static class AuthConfiguration
{
    /// <summary>
    /// Applies authentication entity configurations to the model builder.
    /// </summary>
    public static void ConfigureAuthEntities(this ModelBuilder modelBuilder)
    {
        ConfigureApplicationRole(modelBuilder);
        ConfigureApplicationUserRole(modelBuilder);
        ConfigureRefreshToken(modelBuilder);
    }

    /// <summary>
    /// Configures ApplicationRole entity: table name, keys, indexes, relationships.
    /// </summary>
    private static void ConfigureApplicationRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.HasKey(r => r.Id);
            
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Description).HasMaxLength(500);
            entity.Property(r => r.CreatedAt).IsRequired();

            // Unique constraint on role name
            entity.HasIndex(r => r.Name).IsUnique();

            // One role has many user-role mappings
            entity.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Configures ApplicationUserRole (junction table) entity.
    /// Maps many users to many roles with metadata.
    /// </summary>
    private static void ConfigureApplicationUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUserRole>(entity =>
        {
            entity.HasKey(ur => ur.Id);

            entity.Property(ur => ur.UserId).IsRequired();
            entity.Property(ur => ur.RoleId).IsRequired();
            entity.Property(ur => ur.AssignedAt).IsRequired();

            // Many user-roles map to one user
            entity.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many user-roles map to one role
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite unique constraint: user cannot have same role twice
            entity.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
        });
    }

    /// <summary>
    /// Configures RefreshToken entity: hashed token storage with audit trail.
    /// </summary>
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
            entity.Property(rt => rt.RevokedAt);
            entity.Property(rt => rt.UsedAt);

            // Foreign key to User
            entity.HasOne<RestaurantAPI.Models.User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance: user lookups and expiration checks
            entity.HasIndex(rt => rt.UserId);
            entity.HasIndex(rt => rt.ExpiresAt);
        });
    }
}
