namespace RestaurantAPI.DTOs;

/// <summary>
/// DTO for item response (menu item details)
/// Phase A.8: Naming normalization - *ResponseDTO suffix
/// Used when returning items in menu/search results
/// </summary>
public class ItemResponseDTO
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public decimal ItemPrice { get; set; }
    public string RestaurantName { get; set; }
    public int RestaurantID { get; set; }
    public string ImageUrl { get; set; }
}

/// <summary>
/// Backward compatibility alias for ItemResponseDTO
/// TODO: Remove after Phase B migration
/// </summary>
[Obsolete("Use ItemResponseDTO instead")]
public class GetItemsDTO : ItemResponseDTO
{
}