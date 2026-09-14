namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for cart item responses
/// Phase B.1: Decouples external API contract from internal CartDTO
/// </summary>
public class ExternalCartItemResponse
{
    public int CartID { get; set; }
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public decimal ItemPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// External DTO for cart responses
/// </summary>
public class ExternalCartResponse
{
    public IEnumerable<ExternalCartItemResponse> Items { get; set; }
    public decimal GrandTotal { get; set; }
}

/// <summary>
/// External DTO for cart summary responses
/// </summary>
public class ExternalCartSummaryResponse
{
    public int ItemCount { get; set; }
    public decimal GrandTotal { get; set; }
}
