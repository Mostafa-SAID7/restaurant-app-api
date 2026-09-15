namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// Role entity for role-based access control (RBAC).
/// Represents authorization groups (Customer, RestaurantOwner, Admin, etc.).
/// Users can have multiple roles via ApplicationUserRole junction table.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class ApplicationRole
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Role name (e.g., "Admin", "Customer", "RestaurantOwner").
    /// Must be unique across all roles.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Human-readable role description for admin/documentation.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// When this role was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property (no EF attributes here - EF config in Infrastructure)
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
}
