namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for menu item responses
/// Phase B.1: Decouples external API contract from internal ItemDTO
/// </summary>
public class ExternalItemResponse
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public decimal ItemPrice { get; set; }
    public string RestaurantName { get; set; }
    public int RestaurantID { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// External DTO for menu item list responses with pagination
/// </summary>
public class ExternalItemListResponse
{
    public IEnumerable<ExternalItemResponse> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
