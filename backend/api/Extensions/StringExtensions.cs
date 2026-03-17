using System.Text.RegularExpressions;

namespace FakeRestuarantAPI.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Checks if a string is null or empty
    /// </summary>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Checks if a string is null, empty, or whitespace
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Converts string to title case
    /// </summary>
    public static string ToTitleCase(this string input)
    {
        if (input.IsNullOrEmpty())
            return string.Empty;

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }

    /// <summary>
    /// Validates email format
    /// </summary>
    public static bool IsValidEmail(this string email)
    {
        if (email.IsNullOrEmpty())
            return false;

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }

    /// <summary>
    /// Truncates string to specified length
    /// </summary>
    public static string Truncate(this string input, int maxLength)
    {
        if (input.IsNullOrEmpty() || input.Length <= maxLength)
            return input ?? string.Empty;

        return input.Substring(0, maxLength) + "...";
    }

    /// <summary>
    /// Removes special characters and spaces for file names
    /// </summary>
    public static string ToSafeFileName(this string input)
    {
        if (input.IsNullOrEmpty())
            return string.Empty;

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeFileName = new string(input.Where(c => !invalidChars.Contains(c)).ToArray());
        return safeFileName.Replace(" ", "_");
    }
}