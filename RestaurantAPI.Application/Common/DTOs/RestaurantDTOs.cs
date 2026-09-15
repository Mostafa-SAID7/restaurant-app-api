using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Application.Common.DTOs;

/// <summary>
/// DTO for creating/updating restaurants.
/// </summary>
public class CreateRestaurantDto
{
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
}

/// <summary>
/// DTO for restaurant response.
/// </summary>
public class RestaurantDto
{
    public int RestaurantID { get; set; }
    public string RestaurantName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool ParkingLot { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Request body for uploading a base64-encoded image.
/// </summary>
public class ImageRequestDTO
{
    public string? Base64Image { get; set; }
    public string? FileName { get; set; }
}
