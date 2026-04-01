using FakeRestuarantAPI.Extensions;
using System.Text.RegularExpressions;

namespace FakeRestuarantAPI.Helpers;

public static class ValidationHelper
{
    /// <summary>
    /// Validates restaurant data
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateRestaurant(string name, string address, string type)
    {
        var errors = new List<string>();

        if (name.IsNullOrWhiteSpace())
            errors.Add("Restaurant name is required");
        else if (name.Length > 255)
            errors.Add("Restaurant name cannot exceed 255 characters");

        if (address.IsNullOrWhiteSpace())
            errors.Add("Address is required");
        else if (address.Length > 255)
            errors.Add("Address cannot exceed 255 characters");

        if (type.IsNullOrWhiteSpace())
            errors.Add("Restaurant type is required");

        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates user registration data
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateUserRegistration(string email, string password)
    {
        var errors = new List<string>();

        if (email.IsNullOrWhiteSpace())
            errors.Add("Email is required");
        else if (!email.IsValidEmail())
            errors.Add("Invalid email format");
        else if (email.Length > 500)
            errors.Add("Email cannot exceed 500 characters");

        if (password.IsNullOrWhiteSpace())
            errors.Add("Password is required");
        else if (password.Length < 6)
            errors.Add("Password must be at least 6 characters long");
        else if (password.Length > 30)
            errors.Add("Password cannot exceed 30 characters");

        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates menu item data
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateMenuItem(string name, decimal price, string? description)
    {
        var errors = new List<string>();

        if (name.IsNullOrWhiteSpace())
            errors.Add("Item name is required");
        else if (name.Length > 100)
            errors.Add("Item name cannot exceed 100 characters");

        if (price <= 0)
            errors.Add("Item price must be greater than 0");
        else if (price > 999999.99m)
            errors.Add("Item price is too high");

        if (!description.IsNullOrWhiteSpace() && description.Length > 300)
            errors.Add("Item description cannot exceed 300 characters");

        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates order quantity
    /// </summary>
    public static (bool IsValid, string? Error) ValidateOrderQuantity(int quantity)
    {
        if (quantity < 1)
            return (false, "Quantity must be at least 1");
        
        if (quantity > 100)
            return (false, "Quantity cannot exceed 100");

        return (true, null);
    }

    /// <summary>
    /// Validates API key format
    /// </summary>
    public static bool IsValidApiKey(string? apiKey)
    {
        if (apiKey.IsNullOrWhiteSpace())
            return false;

        // Check if it's a valid GUID format
        return Guid.TryParse(apiKey, out _);
    }

    /// <summary>
    /// Validates pagination parameters
    /// </summary>
    public static (int Page, int PageSize) ValidatePagination(int page, int pageSize)
    {
        var validPage = Math.Max(1, page);
        var validPageSize = Math.Max(1, Math.Min(100, pageSize)); // Max 100 items per page

        return (validPage, validPageSize);
    }

    /// <summary>
    /// Validates sort parameter
    /// </summary>
    public static bool IsValidSortParameter(string? sortBy, string[] allowedFields)
    {
        if (sortBy.IsNullOrWhiteSpace())
            return true; // Empty sort is valid (no sorting)

        var sortField = sortBy.ToLower();
        return allowedFields.Any(field => field.ToLower() == sortField);
    }

    /// <summary>
    /// Validates phone number format
    /// </summary>
    public static bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (phoneNumber.IsNullOrWhiteSpace())
            return false;

        // Remove all non-digit characters
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // Check if it's between 10-15 digits (international format)
        return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
    }

    /// <summary>
    /// Validates URL format
    /// </summary>
    public static bool IsValidUrl(string? url)
    {
        if (url.IsNullOrWhiteSpace())
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) 
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Validates price range
    /// </summary>
    public static bool IsValidPriceRange(decimal minPrice, decimal maxPrice)
    {
        return minPrice >= 0 && maxPrice >= 0 && minPrice <= maxPrice;
    }
}