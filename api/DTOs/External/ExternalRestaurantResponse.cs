namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for restaurant responses
/// Phase B.1: Decouples external API contract from internal RestaurantDTO
/// </summary>
public class ExternalRestaurantResponse
{
    public int RestaurantID { get; set; }
    public string RestaurantName { get; set; }
    public string Address { get; set; }
    public string Type { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// External DTO for restaurant list responses with pagination
/// </summary>
public class ExternalRestaurantListResponse
{
    public IEnumerable<ExternalRestaurantResponse> Restaurants { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
