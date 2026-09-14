using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs.External;

/// <summary>
/// External DTO for base64 image upload requests
/// Phase B.1: Decouples external API contract from internal ImageRequestDTO
/// </summary>
public class ExternalImageUploadRequest
{
    [Required(ErrorMessage = "Base64 encoded image data is required")]
    public string Base64Image { get; set; }

    [StringLength(255, ErrorMessage = "Filename must not exceed 255 characters")]
    public string? FileName { get; set; }
}

/// <summary>
/// External DTO for image upload responses
/// </summary>
public class ExternalImageUploadResponse
{
    public string ImageUrl { get; set; }
    public string ImagePath { get; set; }
    public DateTime UploadedAt { get; set; }
}
