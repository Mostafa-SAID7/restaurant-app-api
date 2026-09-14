using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for restaurant creation/update requests
/// Phase B.1: Decouples external API contract from internal RestaurantDTO
/// </summary>
public class ExternalRestaurantRequest
{
    [Required(ErrorMessage = "Restaurant name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Restaurant name must be between 3 and 100 characters")]
    public string RestaurantName { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(255, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 255 characters")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Type is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Type must be between 3 and 50 characters")]
    public string Type { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
