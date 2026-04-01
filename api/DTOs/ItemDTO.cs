using System.ComponentModel.DataAnnotations;

namespace FakeRestuarantAPI.Models;

public class ItemDTO
{
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; }
    
    [Required]
    public decimal ItemPrice { get; set; }
    
    [MaxLength(300)]
    public string ItemDescription { get; set; }

    public string imageUrl { get; set; }
}