namespace RestaurantAPI.Domain.Entities;

/// <summary>
/// Restaurant entity representing a restaurant business.
/// Pure domain entity with NO EF Core attributes or infrastructure concerns.
/// </summary>
public class Restaurant
{
    /// <summary>
    /// Unique restaurant identifier.
    /// </summary>
    public int RestaurantID { get; set; }

    /// <summary>
    /// Restaurant's business name.
    /// </summary>
    public string RestaurantName { get; set; } = null!;

    /// <summary>
    /// Physical address of the restaurant.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Type/category of restaurant (e.g., "Italian", "Fast Food", "Seafood").
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Whether the restaurant has parking available.
    /// </summary>
    public bool ParkingLot { get; set; }

    /// <summary>
    /// When this restaurant was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this restaurant was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (no EF attributes here - EF config in Infrastructure)
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<MasterOrder> MasterOrders { get; set; } = new List<MasterOrder>();
}
