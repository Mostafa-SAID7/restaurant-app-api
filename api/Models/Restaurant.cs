using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestuarantAPI.Models;

/// <summary>
/// Restaurant entity representing a restaurant business
/// </summary>
public class Restaurant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RestaurantID { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string RestaurantName { get; set; } = null!;
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = null!;

    public bool ParkingLot { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Item>? Items { get; set; } = new List<Item>();
    public ICollection<MasterOrder>? MasterOrders { get; set; } = new List<MasterOrder>();
}
