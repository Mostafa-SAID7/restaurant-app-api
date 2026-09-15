namespace RestaurantAPI.Application.Common.Abstractions;

/// <summary>
/// Port for image storage/retrieval operations.
/// Implementation lives in Infrastructure layer.
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Upload an image and return its URL.
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string folderName);

    /// <summary>
    /// Delete an image by URL.
    /// </summary>
    Task<bool> DeleteAsync(string imageUrl);
}
