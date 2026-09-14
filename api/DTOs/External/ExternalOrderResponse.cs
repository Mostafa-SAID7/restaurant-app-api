namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for order line item responses
/// Phase B.1: Decouples external API contract from internal OrderDTO
/// </summary>
public class ExternalOrderLineResponse
{
    public int OrderID { get; set; }
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public decimal ItemPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// External DTO for order responses
/// </summary>
public class ExternalOrderResponse
{
    public int MasterID { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ExternalOrderLineResponse> Items { get; set; } = new();
}

/// <summary>
/// External DTO for order list responses
/// </summary>
public class ExternalOrderListResponse
{
    public IEnumerable<ExternalOrderResponse> Orders { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
