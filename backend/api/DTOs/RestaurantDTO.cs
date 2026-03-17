using System.ComponentModel.DataAnnotations;

namespace FakeRestuarantAPI.Models;

public class RestaurantDTO
{
    [Required]
    [MaxLength(255)]
    public string RestaurantName { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Address { get; set; }
    
    [Required]
    public string Type { get; set; }

    public bool ParkingLot { get; set; }
}