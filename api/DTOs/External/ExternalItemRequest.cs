using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for menu item creation/update requests
/// Phase B.1: Decouples external API contract from internal ItemDTO
/// </summary>
public class ExternalItemRequest
{
    [Required(ErrorMessage = "Item name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Item name must be between 3 and 100 characters")]
    public string ItemName { get; set; }

    [Required(ErrorMessage = "Item description is required")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Item description must be between 10 and 500 characters")]
    public string ItemDescription { get; set; }

    [Required(ErrorMessage = "Item price is required")]
    [Range(0.01, 99999.99, ErrorMessage = "Item price must be between 0.01 and 99999.99")]
    public decimal ItemPrice { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }
}
