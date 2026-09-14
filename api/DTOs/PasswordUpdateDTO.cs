using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

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
