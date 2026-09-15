namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// Junction table mapping Users to Roles (many-to-many relationship).
/// Enables users to have multiple roles and roles to have multiple users.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class ApplicationUserRole
{
    /// <summary>
    /// Primary key (composite of UserId + RoleId).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to User.
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Foreign key to Role.
    /// </summary>
    public string RoleId { get; set; } = null!;

    /// <summary>
    /// When this role assignment was created.
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties (no EF attributes here - EF config in Infrastructure)
    public User User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}
