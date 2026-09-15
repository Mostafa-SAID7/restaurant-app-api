namespace RestaurantAPI.Application.Common.Abstractions;

/// <summary>
/// Port for image storage/retrieval operations.
/// Implementation lives in Infrastructure layer.
///
/// Uses Stream instead of IFormFile to keep the Application layer free of
/// ASP.NET Core HTTP dependencies (clean architecture boundary).
/// Controllers/Infrastructure convert IFormFile → Stream before calling these methods.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Upload an image stream and return its storage path.
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string folderName);

    /// <summary>
    /// Delete an image by path/URL.
    /// </summary>
    Task<bool> DeleteAsync(string imageUrl);

    /// <summary>
    /// Save an image from a stream and return its path.
    /// Callers (e.g. controllers) should open the IFormFile stream and pass it here.
    /// </summary>
    Task<string> SaveImageAsync(Stream fileStream, string fileName);

    /// <summary>
    /// Save a base64-encoded image and return its path.
    /// </summary>
    Task<string> SaveBase64ImageAsync(string base64Data, string fileName);

    /// <summary>
    /// Get the public URL for an image given its path.
    /// </summary>
    string GetImageUrl(string imagePath);
}
