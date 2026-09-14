namespace RestaurantAPI.DTOs;

/// <summary>
/// Unified API response contract for all endpoints
/// Phase A.4: Centralized response model (SRP)
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the operation succeeded
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Human-readable message about the operation
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Response data (null if error or no data to return)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Additional error details (for failed requests)
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// Validation errors (for validation failures)
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Pagination metadata (for paginated responses)
    /// </summary>
    public PaginationMetadata? Pagination { get; set; }

    /// <summary>
    /// Timestamp when response was generated (UTC)
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ApiResponse()
    {
        Timestamp = DateTime.UtcNow;
        Message = string.Empty;
    }

    /// <summary>
    /// Factory method for successful response with data
    /// </summary>
    public static ApiResponse<T> CreateSuccess(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for error response
    /// </summary>
    public static ApiResponse<T> CreateError(string message, object? details = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for validation error response
    /// </summary>
    public static ApiResponse<T> CreateValidationError(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Pagination metadata for paginated responses
/// </summary>
public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
