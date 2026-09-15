using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Application.Common.DTOs;

/// <summary>
/// DTO for a single order line item (input).
/// </summary>
public class OrderLineInputDto
{
    public int? ItemID { get; set; }

    [MaxLength(100)]
    public string? ItemName { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}

/// <summary>
/// DTO for creating an order (input).
/// </summary>
public class CreateOrderDto
{
    [Required(ErrorMessage = "Order lines are required")]
    [MinLength(1, ErrorMessage = "At least one item must be ordered")]
    public List<OrderLineInputDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for order line item in response.
/// </summary>
public class OrderLineDto
{
    public int OrderID { get; set; }
    public int ItemID { get; set; }
    public string ItemName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal ItemPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int MasterID { get; set; }
}

/// <summary>
/// DTO for order creation response.
/// </summary>
public class CreateOrderResponseDto
{
    public int MasterID { get; set; }
    public List<OrderLineDto> Items { get; set; } = new();
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for master order (order header).
/// </summary>
public class MasterOrderDto
{
    public int MasterID { get; set; }
    public string UserID { get; set; } = null!;
    public int RestaurantID { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for master order with line items (response).
/// </summary>
public class MasterOrderWithItemsDto
{
    public int MasterID { get; set; }
    public string UserCode { get; set; } = null!;
    public int RestaurantID { get; set; }
    public string RestaurantName { get; set; } = null!;
    public decimal GrandTotal { get; set; }
    public List<OrderLineDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
