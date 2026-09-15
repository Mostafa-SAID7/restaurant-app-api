using RestaurantAPI.Application.Common.Abstractions;
using Microsoft.AspNetCore.Http;

namespace RestaurantAPI.Infrastructure.Services;

/// <summary>
/// Implementation of IImageService for image upload/deletion.
/// Currently uses in-memory storage. In production, replace with cloud storage (Azure Blob, S3, etc.).
/// </summary>
public class ImageService : IImageService
{
    private readonly ILogger<ImageService> _logger;
    private static readonly Dictionary<string, byte[]> _imageStore = new();

    public ImageService(ILogger<ImageService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Uploads an image and returns its URL (path).
    /// </summary>
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string folderName)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var imageData = memoryStream.ToArray();

            var imagePath = $"{folderName}/{fileName}";
            _imageStore[imagePath] = imageData;

            _logger.LogInformation("Image uploaded: {ImagePath}", imagePath);
            return imagePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            throw;
        }
    }

    /// <summary>
    /// Deletes an image by URL/path.
    /// </summary>
    public async Task<bool> DeleteAsync(string imageUrl)
    {
        try
        {
            if (_imageStore.Remove(imageUrl))
            {
                _logger.LogInformation("Image deleted: {ImageUrl}", imageUrl);
                return true;
            }

            _logger.LogWarning("Image not found: {ImageUrl}", imageUrl);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image");
            throw;
        }
    }

    /// <summary>
    /// Saves an IFormFile and returns the image path.
    /// Helper method for direct file upload.
    /// </summary>
    public async Task<string> SaveImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        using var stream = file.OpenReadStream();
        var imagePath = await UploadAsync(stream, file.FileName, "images");
        return imagePath;
    }

    /// <summary>
    /// Saves a base64-encoded image and returns the image path.
    /// Helper method for base64 image upload.
    /// </summary>
    public async Task<string> SaveBase64ImageAsync(string base64Data, string fileName)
    {
        if (string.IsNullOrEmpty(base64Data))
            throw new ArgumentException("Base64 data is empty");

        try
        {
            // Remove data:image/png;base64, prefix if present
            var base64String = base64Data.Contains(",") ? base64Data.Split(',')[1] : base64Data;
            var imageBytes = Convert.FromBase64String(base64String);

            using var stream = new MemoryStream(imageBytes);
            var imagePath = await UploadAsync(stream, fileName, "images/base64");
            return imagePath;
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Invalid base64 data");
            throw new ArgumentException("Invalid base64 image data", ex);
        }
    }

    /// <summary>
    /// Gets the image URL from a path (identity function for now).
    /// In production, this would generate a proper URL (e.g., CDN URL).
    /// </summary>
    public string GetImageUrl(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return null!;

        // In production, construct full URL: https://cdn.example.com/imagePath
        return $"/api/images/{imagePath}";
    }
}
