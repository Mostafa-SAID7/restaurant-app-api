using System.ComponentModel.DataAnnotations;

namespace RestuarantAPI.Models;

/// <summary>
/// User entity representing a registered user
/// </summary>
public class User
{
    [Key]
    public string Usercode { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    [EmailAddress]
    public string UserEmail { get; set; } = null!;

    /// <summary>
    /// Hashed password using bcrypt or PBKDF2
    /// Never store or transmit plaintext passwords
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Cart>? Carts { get; set; } = new List<Cart>();
    public ICollection<Order>? Orders { get; set; } = new List<Order>();
    public ICollection<MasterOrder>? MasterOrders { get; set; } = new List<MasterOrder>();
}
