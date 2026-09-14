using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for cart item add/update requests
/// Phase B.1: Decouples external API contract from internal AddCartItemRequestDTO
/// </summary>
public class ExternalCartItemRequest
{
    [Required(ErrorMessage = "ItemID is required")]
    public int ItemID { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}
