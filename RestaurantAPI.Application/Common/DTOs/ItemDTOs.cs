using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Application.Common.DTOs;

/// <summary>
/// DTO for creating/updating menu items.
/// </summary>
public class CreateItemDto
{
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = null!;

    [Required]
    [Range(0.01, 999999.99)]
    public decimal ItemPrice { get; set; }

    [MaxLength(500)]
    public string? ItemDescription { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }
}

/// <summary>
/// DTO for menu item response.
/// </summary>
public class ItemDto
{
    public int ItemID { get; set; }
    public string ItemName { get; set; } = null!;
    public string? ItemDescription { get; set; }
    public decimal ItemPrice { get; set; }
    public string? ImageUrl { get; set; }
    public int RestaurantID { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
