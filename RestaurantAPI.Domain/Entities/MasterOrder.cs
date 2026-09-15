namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// MasterOrder entity representing the summary/header of a user's order at a restaurant.
/// Contains overall order metadata; individual items are stored in Order entities.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class MasterOrder
{
    /// <summary>
    /// Unique master order identifier.
    /// </summary>
    public int MasterID { get; set; }

    /// <summary>
    /// Foreign key to the user who placed this order.
    /// </summary>
    public string UserID { get; set; } = null!;

    /// <summary>
    /// Foreign key to the restaurant this order is from.
    /// </summary>
    public int RestaurantID { get; set; }

    /// <summary>
    /// Total amount for the entire order (sum of all line items).
    /// </summary>
    public decimal GrandTotal { get; set; }

    /// <summary>
    /// When this order was created/placed.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this order was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (no EF attributes here - EF config in Infrastructure)
    public User User { get; set; } = null!;
    public Restaurant Restaurant { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
