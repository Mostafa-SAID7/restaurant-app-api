namespace RestuarantAPI.Services.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile imageFile, string folder = "items");
    Task<bool> DeleteImageAsync(string imagePath);
    string GetImageUrl(string imagePath);
    Task<string> SaveBase64ImageAsync(string base64Image, string fileName, string folder = "items");
}