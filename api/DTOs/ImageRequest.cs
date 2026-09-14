using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

/// <summary>
/// DTO for uploading base64 encoded images
/// Phase A.8: Naming normalization - *RequestDTO suffix
/// </summary>
public class ImageRequestDTO
{
    [Required]
    public string Base64Image { get; set; }
    
    [MaxLength(255)]
    public string? FileName { get; set; }
}

/// <summary>
/// Backward compatibility alias for ImageRequestDTO
/// TODO: Remove after Phase B migration
/// </summary>
[Obsolete("Use ImageRequestDTO instead")]
public class ImageRequest : ImageRequestDTO
{
}