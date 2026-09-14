using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

/// <summary>
/// DTO for creating an order with multiple line items
/// Phase A.8: Naming normalization - CreateOrderRequestDTO for clarity
/// </summary>
public class CreateOrderRequestDTO
{
    [Required(ErrorMessage = "Menu items are required")]
    [MinLength(1, ErrorMessage = "At least one item must be in the menu")]
    public List<OrderDTO> Items { get; set; } = new();
}

/// <summary>
/// Backward compatibility alias for CreateOrderRequestDTO
/// TODO: Remove after Phase B migration
/// </summary>
[Obsolete("Use CreateOrderRequestDTO instead")]
public class MenuDTO : CreateOrderRequestDTO
{
    /// <summary>
    /// Backward compatibility property name
    /// </summary>
    [Obsolete("Use Items instead")]
    public List<OrderDTO> menuDTO 
    { 
        get => Items; 
        set => Items = value; 
    }
}