using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

public class UserDTO
{
    [Required]
    [MaxLength(500)]
    [EmailAddress]
    public string UserEmail { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Password { get; set; }
}