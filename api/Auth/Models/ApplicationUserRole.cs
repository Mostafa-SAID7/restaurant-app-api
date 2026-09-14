using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantAPI.Models;

namespace RestaurantAPI.Auth.Models;

/// <summary>
/// Junction table mapping Users to Roles (many-to-many relationship).
/// Enables users to have multiple roles and roles to have multiple users.
/// Replaces the default AspNetUserRoles from Identity if not using full Identity.
/// </summary>
[Table("UserRoles")]
public class ApplicationUserRole
{
    /// <summary>
    /// Primary key (composite of UserId + RoleId).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to User.
    /// </summary>
    [Required]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Foreign key to Role.
    /// </summary>
    [Required]
    public string RoleId { get; set; } = null!;

    /// <summary>
    /// When this role assignment was created.
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property: User.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Navigation property: Role.
    /// </summary>
    public ApplicationRole Role { get; set; } = null!;
}
