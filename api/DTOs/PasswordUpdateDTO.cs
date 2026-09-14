using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models;

/// <summary>
/// DTO for password update requests
/// </summary>
public class PasswordUpdateDTO
{
    [Required]
    [MinLength(6)]
    [MaxLength(30)]
    public string NewPassword { get; set; } = null!;
}
