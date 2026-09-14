using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Auth.Models;

/// <summary>
/// Role entity for role-based access control (RBAC).
/// Represents authorization groups (Customer, RestaurantOwner, Admin, etc.).
/// Users can have multiple roles via UserRole junction table.
/// </summary>
[Table("Roles")]
public class ApplicationRole
{
    /// <summary>
    /// Primary key.
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Role name (e.g., "Admin", "Customer", "RestaurantOwner").
    /// Must be unique across all roles.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Human-readable role description for admin/documentation.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// When this role was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property: Users assigned to this role.
    /// </summary>
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
}
