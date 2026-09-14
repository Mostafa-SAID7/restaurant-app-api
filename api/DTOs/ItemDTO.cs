using System.ComponentModel.DataAnnotations;

namespace RestuarantAPI.Models;

/// <summary>
/// Data Transfer Object for creating/updating menu items
/// </summary>
public class ItemDTO
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