namespace RestaurantAPI.DTOs;

/// <summary>
/// Enhanced API response wrapper with metadata and request tracking
/// Phase B.2: Adds request ID, API version, and standardized metadata
/// Wraps ApiResponse<T> for consistent response envelopes across all endpoints
/// </summary>
public class ApiResponseWrapper<T>
{
    /// <summary>
    /// Unique request identifier for tracing
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// API version
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Timestamp when response was generated (UTC)
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Response execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// The wrapped API response
    /// </summary>
    public ApiResponse<T> Response { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ApiResponseWrapper()
    {
        Timestamp = DateTime.UtcNow;
        RequestId = Guid.NewGuid().ToString("N")[..12]; // First 12 chars of GUID
    }

    /// <summary>
    /// Factory method for successful response
    /// </summary>
    public static ApiResponseWrapper<T> CreateSuccess(T data, string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponseWrapper<T>
        {
            StatusCode = statusCode,
            Response = ApiResponse<T>.CreateSuccess(data, message),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for created response (201)
    /// </summary>
    public static ApiResponseWrapper<T> CreateCreated(T data, string message = "Resource created successfully")
    {
        return CreateSuccess(data, message, 201);
    }

    /// <summary>
    /// Factory method for error response
    /// </summary>
    public static ApiResponseWrapper<T> CreateError(string message, int statusCode = 400, object? details = null)
    {
        return new ApiResponseWrapper<T>
        {
            StatusCode = statusCode,
            Response = ApiResponse<T>.CreateError(message, details),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for validation error response (422)
    /// </summary>
    public static ApiResponseWrapper<T> CreateValidationError(List<string> errors)
    {
        return new ApiResponseWrapper<T>
        {
            StatusCode = 422,
            Response = ApiResponse<T>.CreateValidationError(errors),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for not found response (404)
    /// </summary>
    public static ApiResponseWrapper<T> CreateNotFound(string message = "Resource not found")
    {
        return CreateError(message, 404);
    }

    /// <summary>
    /// Factory method for unauthorized response (401)
    /// </summary>
    public static ApiResponseWrapper<T> CreateUnauthorized(string message = "Unauthorized access")
    {
        return CreateError(message, 401);
    }

    /// <summary>
    /// Factory method for forbidden response (403)
    /// </summary>
    public static ApiResponseWrapper<T> CreateForbidden(string message = "Access forbidden")
    {
        return CreateError(message, 403);
    }

    /// <summary>
    /// Sets execution time (called at end of request)
    /// </summary>
    public void SetExecutionTime(long milliseconds)
    {
        ExecutionTimeMs = milliseconds;
    }
}

/// <summary>
/// Non-generic version for responses without data
/// </summary>
public class ApiResponseWrapper
{
    /// <summary>
    /// Unique request identifier for tracing
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// API version
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Timestamp when response was generated (UTC)
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Response execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Indicates success
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ApiResponseWrapper()
    {
        Timestamp = DateTime.UtcNow;
        RequestId = Guid.NewGuid().ToString("N")[..12];
    }

    /// <summary>
    /// Factory method for successful response
    /// </summary>
    public static ApiResponseWrapper CreateSuccess(string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponseWrapper
        {
            Success = true,
            Message = message,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for error response
    /// </summary>
    public static ApiResponseWrapper CreateError(string message, int statusCode = 400)
    {
        return new ApiResponseWrapper
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }
}
