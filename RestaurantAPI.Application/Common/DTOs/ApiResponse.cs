namespace RestaurantAPI.Application.Common.DTOs;

/// <summary>
/// Unified API response contract for all endpoints.
/// Used by WebApi layer to wrap handler results.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Details { get; set; }
    public List<string>? Errors { get; set; }
    public PaginationMetadata? Pagination { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> CreateSuccess(T data, string message = "Operation completed successfully")
        => new() 
        { 
            Success = true, 
            Data = data, 
            Message = message, 
            Timestamp = DateTime.UtcNow 
        };

    public static ApiResponse<T> CreateError(string message, object? details = null)
        => new() 
        { 
            Success = false, 
            Message = message, 
            Details = details, 
            Timestamp = DateTime.UtcNow 
        };

    public static ApiResponse<T> CreateValidationError(List<string> errors)
        => new() 
        { 
            Success = false, 
            Message = "Validation failed", 
            Errors = errors, 
            Timestamp = DateTime.UtcNow 
        };
}

public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
