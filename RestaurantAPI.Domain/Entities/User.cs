namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// User entity representing a registered user.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier (user code/ID).
    /// </summary>
    public string Usercode { get; set; } = null!;

    /// <summary>
    /// User's email address - used for authentication.
    /// </summary>
    public string UserEmail { get; set; } = null!;

    /// <summary>
    /// Hashed password using bcrypt or PBKDF2.
    /// Never store or transmit plaintext passwords.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// When this user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this user was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (no EF attributes here - EF config in Infrastructure)
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<MasterOrder> MasterOrders { get; set; } = new List<MasterOrder>();
}
