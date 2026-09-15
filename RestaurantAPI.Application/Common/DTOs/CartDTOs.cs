using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Application.Common.DTOs;

/// <summary>
/// DTO for adding an item to the cart.
/// </summary>
public class AddCartItemDto
{
    [Required]
    public int ItemID { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO for a single cart item in response.
/// </summary>
public class CartItemDto
{
    public int CartID { get; set; }
    public int ItemID { get; set; }
    public string ItemName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal ItemPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// DTO for complete cart response.
/// </summary>
public class CartDto
{
    public List<CartItemDto> CartItems { get; set; } = new();
    public decimal GrandTotal { get; set; }
}
