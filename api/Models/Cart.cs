using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Models;

/// <summary>
/// Cart entity representing items in a user's shopping cart
/// </summary>
public class Cart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CartID { get; set; }

    [ForeignKey(nameof(User))]
    public string UserID { get; set; } = null!;

    [Required]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(Item))]
    public int ItemID { get; set; }

    [Required]
    public Item Item { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = null!;

    [Range(1, 100)]
    public int Quantity { get; set; }

    [Precision(10, 2)]
    [Range(0.01, 999999.99)]
    public decimal ItemPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
