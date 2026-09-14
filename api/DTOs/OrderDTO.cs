using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.DTOs;

/// <summary>
/// DTO for a single order line item (input)
/// </summary>
public class OrderDTO
{
    /// <summary>
    /// Item ID (preferred) or ItemName (for backward compatibility)
    /// </summary>
    public int? ItemID { get; set; }

    [MaxLength(100)]
    public string ItemName { get; set; }
    
    [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO for order line item with full details (used internally)
/// </summary>
public class OrderLineDTO
{
    public int OrderID { get; set; }
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }

    [Precision(10, 2)]
    public decimal ItemPrice { get; set; }

    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    public int MasterID { get; set; }
}

/// <summary>
/// DTO for creating an order (input)
/// </summary>
public class CreateOrderDTO
{
    [Required(ErrorMessage = "Order lines are required")]
    [MinLength(1, ErrorMessage = "At least one item must be ordered")]
    public List<OrderDTO> Items { get; set; } = new();
}

/// <summary>
/// DTO for order response (no entity exposure)
/// </summary>
public class OrderResponseDTO
{
    public int MasterID { get; set; }
    public List<OrderLineDTO> Items { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for master order (order header)
/// </summary>
public class MasterOrderDTO
{
    public int MasterID { get; set; }
    public string UserID { get; set; }
    public int RestaurantID { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for master order with line items (response)
/// </summary>
public class MasterOrderWithItemsDTO
{
    public int MasterID { get; set; }
    public string UserCode { get; set; }
    public int RestaurantID { get; set; }
    public string RestaurantName { get; set; }
    public decimal GrandTotal { get; set; }
    public List<OrderLineDTO> Items { get; set; }
    public DateTime CreatedAt { get; set; }
}
