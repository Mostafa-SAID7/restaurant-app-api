using Microsoft.AspNetCore.Http;

namespace RestaurantAPI.Application.Common.Abstractions;

/// <summary>
/// Port for image storage/retrieval operations.
/// Implementation lives in Infrastructure layer.
/// Supports direct file upload, base64 encoding, and image retrieval.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Upload an image stream and return its path.
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string folderName);

    /// <summary>
    /// Delete an image by path/URL.
    /// </summary>
    Task<bool> DeleteAsync(string imageUrl);

    /// <summary>
    /// Save an uploaded IFormFile and return its storage path.
    /// Previously declared as (object file) which did not match the implementation — fixed.
    /// </summary>
    /// <param name="file">Uploaded file from a multipart/form-data request</param>
    Task<string> SaveImageAsync(IFormFile file);

    /// <summary>
    /// Save a base64-encoded image and return its path.
    /// </summary>
    Task<string> SaveBase64ImageAsync(string base64Data, string fileName);

    /// <summary>
    /// Get the public URL for an image given its path.
    /// </summary>
    string GetImageUrl(string imagePath);
}
