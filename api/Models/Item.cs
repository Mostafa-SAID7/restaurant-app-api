using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Models;

/// <summary>
/// Item entity representing a menu item in a restaurant
/// </summary>
public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ItemID { get; set; }

    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = null!;

    [MaxLength(500)]
    public string? ItemDescription { get; set; }

    [Precision(10, 2)]
    [Range(0.01, 999999.99)]
    public decimal ItemPrice { get; set; }

    [ForeignKey(nameof(Restaurant))]
    public int RestaurantID { get; set; }

    [Required]
    public Restaurant Restaurant { get; set; } = null!;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Cart>? Carts { get; set; } = new List<Cart>();
    public ICollection<Order>? Orders { get; set; } = new List<Order>();
}
