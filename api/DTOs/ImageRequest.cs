using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

public class ImageRequest
{
    [Required]
    public string Base64Image { get; set; }
    
    [MaxLength(255)]
    public string? FileName { get; set; }
}