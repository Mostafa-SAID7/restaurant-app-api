using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Models;

/// <summary>
/// MasterOrder entity representing the summary of a user's order at a restaurant
/// </summary>
public class MasterOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MasterID { get; set; }

    [ForeignKey(nameof(User))]
    public string UserID { get; set; } = null!;

    [Required]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(Restaurant))]
    public int RestaurantID { get; set; }

    [Required]
    public Restaurant Restaurant { get; set; } = null!;

    [Precision(10, 2)]
    [Range(0.01, 999999.99)]
    public decimal GrandTotal { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Order>? Orders { get; set; } = new List<Order>();
}
