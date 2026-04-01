using FakeRestuarantAPI.Services.Interfaces;
using FakeRestuarantAPI.Helpers;

namespace FakeRestuarantAPI.Services.Implementation;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public ImageService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public async Task<string> SaveImageAsync(IFormFile imageFile, string folder = "items")
    {
        if (!FileHelper.IsValidImage(imageFile))
            throw new ArgumentException("Invalid image file");

        // Create uploads directory if it doesn't exist
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
        FileHelper.EnsureDirectoryExists(uploadsPath);

        // Generate unique filename
        var fileName = FileHelper.GenerateUniqueFileName(imageFile.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // Return relative path
        return Path.Combine("uploads", folder, fileName).Replace("\\", "/");
    }

    public async Task<bool> DeleteImageAsync(string imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
            return FileHelper.SafeDeleteFile(fullPath);
        }
        catch
        {
            return false;
        }
    }

    public string GetImageUrl(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return "/images/default-item.jpg"; // Default image

        return $"/{imagePath}";
    }

    public async Task<string> SaveBase64ImageAsync(string base64Image, string fileName, string folder = "items")
    {
        if (string.IsNullOrEmpty(base64Image))
            throw new ArgumentException("Invalid base64 image");

        // Remove data:image/jpeg;base64, prefix if present
        var base64Data = base64Image.Contains(",") ? base64Image.Split(',')[1] : base64Image;
        var imageBytes = Convert.FromBase64String(base64Data);

        // Create uploads directory if it doesn't exist
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
        FileHelper.EnsureDirectoryExists(uploadsPath);

        // Generate filename with extension
        var fileExtension = GetExtensionFromBase64(base64Image);
        var uniqueFileName = FileHelper.GenerateUniqueFileName($"{fileName}{fileExtension}");
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        // Save file
        await File.WriteAllBytesAsync(filePath, imageBytes);

        // Return relative path
        return Path.Combine("uploads", folder, uniqueFileName).Replace("\\", "/");
    }

    private string GetExtensionFromBase64(string base64Image)
    {
        if (base64Image.StartsWith("data:image/jpeg") || base64Image.StartsWith("data:image/jpg"))
            return ".jpg";
        if (base64Image.StartsWith("data:image/png"))
            return ".png";
        if (base64Image.StartsWith("data:image/gif"))
            return ".gif";
        if (base64Image.StartsWith("data:image/webp"))
            return ".webp";
        
        return ".jpg"; // Default
    }
}