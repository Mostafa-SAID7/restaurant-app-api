using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.DTOs;

/// <summary>
/// DTO for cart response (no entity exposure)
/// </summary>
public class CartDTO
{
    public List<CartItemDTO> cartitems { get; set; }
    public decimal GrandTotal { get; set; }
}

/// <summary>
/// DTO for individual cart item
/// </summary>
public class CartItemDTO
{
    public int CartID { get; set; }
    public int ItemID { get; set; }
    public string ItemName { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
    
    [Precision(10, 2)]
    public decimal ItemPrice { get; set; }

    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// DTO for adding item to cart (input)
/// Simplified to use ItemID instead of embedding Item entity
/// </summary>
public class SetCart
{
    [Required(ErrorMessage = "ItemID is required")]
    public int ItemID { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}