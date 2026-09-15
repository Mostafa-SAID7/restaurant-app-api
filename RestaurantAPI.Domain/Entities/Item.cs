namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// Item entity representing a menu item in a restaurant.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class Item
{
    /// <summary>
    /// Unique item identifier.
    /// </summary>
    public int ItemID { get; set; }

    /// <summary>
    /// Name of the menu item.
    /// </summary>
    public string ItemName { get; set; } = null!;

    /// <summary>
    /// Description of the item.
    /// </summary>
    public string? ItemDescription { get; set; }

    /// <summary>
    /// Price of the item.
    /// </summary>
    public decimal ItemPrice { get; set; }

    /// <summary>
    /// Foreign key to the restaurant this item belongs to.
    /// </summary>
    public int RestaurantID { get; set; }

    /// <summary>
    /// Image URL for the menu item.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// When this item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this item was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (no EF attributes here - EF config in Infrastructure)
    public Restaurant Restaurant { get; set; } = null!;
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
