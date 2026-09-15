namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// Order entity representing individual items in a user's order.
/// Each order line is tied to a MasterOrder (header).
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class Order
{
    /// <summary>
    /// Unique order line identifier.
    /// </summary>
    public int OrderID { get; set; }

    /// <summary>
    /// Foreign key to the user who placed this order.
    /// </summary>
    public string UserID { get; set; } = null!;

    /// <summary>
    /// Foreign key to the item ordered.
    /// </summary>
    public int ItemID { get; set; }

    /// <summary>
    /// Snapshot of the item name at order time.
    /// </summary>
    public string ItemName { get; set; } = null!;

    /// <summary>
    /// Quantity ordered.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Snapshot of the item price at order time.
    /// </summary>
    public decimal ItemPrice { get; set; }

    /// <summary>
    /// Total price for this line (ItemPrice * Quantity).
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Foreign key to the MasterOrder (header) this line belongs to.
    /// </summary>
    public int MasterID { get; set; }

    /// <summary>
    /// When this order line was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties (no EF attributes here - EF config in Infrastructure)
    public User User { get; set; } = null!;
    public Item Item { get; set; } = null!;
    public MasterOrder MasterOrder { get; set; } = null!;
}
