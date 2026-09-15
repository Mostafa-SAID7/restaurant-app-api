namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// Cart entity representing items in a user's shopping cart.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class Cart
{
    /// <summary>
    /// Unique cart item identifier.
    /// </summary>
    public int CartID { get; set; }

    /// <summary>
    /// Foreign key to the user who owns this cart item.
    /// </summary>
    public string UserID { get; set; } = null!;

    /// <summary>
    /// Foreign key to the item in the cart.
    /// </summary>
    public int ItemID { get; set; }

    /// <summary>
    /// Snapshot of the item name at cart add time.
    /// </summary>
    public string ItemName { get; set; } = null!;

    /// <summary>
    /// Quantity of this item in the cart.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Snapshot of the item price at cart add time.
    /// </summary>
    public decimal ItemPrice { get; set; }

    /// <summary>
    /// When this cart item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this cart item was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (no EF attributes here - EF config in Infrastructure)
    public User User { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
