using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for order line item requests
/// Phase B.1: Decouples external API contract from internal OrderDTO
/// </summary>
public class ExternalOrderLineRequest
{
    [Required(ErrorMessage = "ItemID or ItemName is required")]
    public int? ItemID { get; set; }

    [StringLength(100)]
    public string? ItemName { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}

/// <summary>
/// External DTO for order creation requests
/// </summary>
public class ExternalCreateOrderRequest
{
    [Required(ErrorMessage = "Order items are required")]
    [MinLength(1, ErrorMessage = "At least one item must be in the order")]
    public List<ExternalOrderLineRequest> Items { get; set; } = new();
}
