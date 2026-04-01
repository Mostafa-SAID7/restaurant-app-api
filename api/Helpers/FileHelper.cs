using RestuarantAPI.Extensions;

namespace RestuarantAPI.Helpers;

public static class FileHelper
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly long MaxFileSize = 5 * 1024 * 1024; // 5MB
    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" },
        { ".pdf", "application/pdf" },
        { ".txt", "text/plain" },
        { ".json", "application/json" },
        { ".xml", "application/xml" },
        { ".csv", "text/csv" },
        { ".zip", "application/zip" }
    };

    /// <summary>
    /// Validates if file is a valid image
    /// </summary>
    public static bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > MaxFileSize)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLower();
        return AllowedImageExtensions.Contains(extension);
    }

    /// <summary>
    /// Gets file extension from content type
    /// </summary>
    public static string GetExtensionFromContentType(string contentType)
    {
        return contentType.ToLower() switch
        {
            "image/jpeg" => ".jpg",
            "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
    }

    /// <summary>
    /// Generates unique file name
    /// </summary>
    public static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var safeFileName = nameWithoutExtension.ToSafeFileName();
        
        return $"{safeFileName}_{Guid.NewGuid()}{extension}";
    }

    /// <summary>
    /// Gets MIME type from file extension
    /// </summary>
    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return MimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";
    }

    /// <summary>
    /// Ensures directory exists
    /// </summary>
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// Deletes file safely
    /// </summary>
    public static bool SafeDeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets file size in human readable format
    /// </summary>
    public static string GetFileSizeString(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        
        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Validates file size
    /// </summary>
    public static bool IsValidFileSize(IFormFile file, long maxSizeBytes = 5242880) // 5MB default
    {
        return file != null && file.Length > 0 && file.Length <= maxSizeBytes;
    }
}