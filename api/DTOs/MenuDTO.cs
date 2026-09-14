using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

/// <summary>
/// DTO for menu/checkout request (order creation)
/// </summary>
public class MenuDTO
{
    [Required(ErrorMessage = "Menu items are required")]
    [MinLength(1, ErrorMessage = "At least one item must be in the menu")]
    public List<OrderDTO> menuDTO { get; set; } = new();
}