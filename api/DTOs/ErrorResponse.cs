namespace RestaurantAPI.DTOs;

/// <summary>
/// Standardized error code enumeration
/// Phase B.5: Consistent error codes across all endpoints
/// </summary>
public enum ErrorCode
{
    // 400 - Bad Request
    InvalidRequest = 400,
    ValidationError = 4001,
    MissingRequiredField = 4002,
    InvalidFormat = 4003,

    // 401 - Unauthorized
    Unauthorized = 401,
    InvalidCredentials = 4011,
    TokenExpired = 4012,
    TokenInvalid = 4013,

    // 403 - Forbidden
    Forbidden = 403,
    InsufficientPermissions = 4031,
    ResourceAccessDenied = 4032,

    // 404 - Not Found
    NotFound = 404,
    ResourceNotFound = 4041,
    EndpointNotFound = 4042,

    // 409 - Conflict
    Conflict = 409,
    ResourceAlreadyExists = 4091,
    ConcurrencyConflict = 4092,

    // 422 - Unprocessable Entity
    UnprocessableEntity = 422,
    BusinessRuleViolation = 4221,

    // 429 - Too Many Requests
    TooManyRequests = 429,
    RateLimitExceeded = 4291,

    // 500 - Internal Server Error
    InternalError = 500,
    DatabaseError = 5001,
    ServiceUnavailable = 5002,
    ConfigurationError = 5003
}

/// <summary>
/// Standardized error response structure
/// Phase B.5: Consistent error format for all API errors
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Unique error code for programmatic handling
    /// </summary>
    public ErrorCode ErrorCode { get; set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Machine-readable error type
    /// </summary>
    public string ErrorType { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional details (context-specific)
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// Validation errors (if applicable)
    /// </summary>
    public List<ValidationError>? ValidationErrors { get; set; }

    /// <summary>
    /// Stack trace (development only)
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Timestamp when error occurred (UTC)
    /// </summary>
    public DateTime Timestamp { get; set; }

    public ErrorResponse()
    {
        Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method for validation errors
    /// </summary>
    public static ErrorResponse ValidationFailed(List<ValidationError> errors, string message = "One or more validation errors occurred")
    {
        return new ErrorResponse
        {
            ErrorCode = ErrorCode.ValidationError,
            StatusCode = 422,
            ErrorType = "VALIDATION_ERROR",
            Message = message,
            ValidationErrors = errors,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for not found errors
    /// </summary>
    public static ErrorResponse ResourceNotFound(string resourceType, object? identifier = null)
    {
        var message = identifier != null 
            ? $"{resourceType} with identifier '{identifier}' was not found"
            : $"{resourceType} not found";

        return new ErrorResponse
        {
            ErrorCode = ErrorCode.ResourceNotFound,
            StatusCode = 404,
            ErrorType = "NOT_FOUND",
            Message = message,
            Details = new { ResourceType = resourceType, Identifier = identifier },
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for unauthorized errors
    /// </summary>
    public static ErrorResponse Unauthorized(string message = "Unauthorized access")
    {
        return new ErrorResponse
        {
            ErrorCode = ErrorCode.Unauthorized,
            StatusCode = 401,
            ErrorType = "UNAUTHORIZED",
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for forbidden errors
    /// </summary>
    public static ErrorResponse Forbidden(string message = "Access forbidden")
    {
        return new ErrorResponse
        {
            ErrorCode = ErrorCode.Forbidden,
            StatusCode = 403,
            ErrorType = "FORBIDDEN",
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for conflict errors
    /// </summary>
    public static ErrorResponse Conflict(string message, object? details = null)
    {
        return new ErrorResponse
        {
            ErrorCode = ErrorCode.Conflict,
            StatusCode = 409,
            ErrorType = "CONFLICT",
            Message = message,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for rate limit errors
    /// </summary>
    public static ErrorResponse RateLimitExceeded(int retryAfterSeconds)
    {
        return new ErrorResponse
        {
            ErrorCode = ErrorCode.RateLimitExceeded,
            StatusCode = 429,
            ErrorType = "RATE_LIMIT_EXCEEDED",
            Message = $"Rate limit exceeded. Please retry after {retryAfterSeconds} seconds",
            Details = new { RetryAfterSeconds = retryAfterSeconds },
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for internal server errors
    /// </summary>
    public static ErrorResponse InternalError(string message = "An unexpected error occurred", string? stackTrace = null)
    {
        return new ErrorResponse
        {
            ErrorCode = ErrorCode.InternalError,
            StatusCode = 500,
            ErrorType = "INTERNAL_ERROR",
            Message = message,
            StackTrace = stackTrace,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Factory method for business rule violations
    /// </summary>
    public static ErrorResponse BusinessRuleViolation(string message, object? details = null)
    {
        return new ErrorResponse
        {
            ErrorCode = ErrorCode.BusinessRuleViolation,
            StatusCode = 422,
            ErrorType = "BUSINESS_RULE_VIOLATION",
            Message = message,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Individual validation error
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Field name that failed validation
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Error message for this field
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Attempted value (optional)
    /// </summary>
    public object? AttemptedValue { get; set; }

    public ValidationError(string field, string message, object? attemptedValue = null)
    {
        Field = field;
        Message = message;
        AttemptedValue = attemptedValue;
    }
}

/// <summary>
/// Standardized error response wrapper
/// Phase B.5: Wraps ErrorResponse for consistent API format
/// </summary>
public class StandardizedErrorResponse
{
    /// <summary>
    /// Request ID for tracing
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// API version
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// Error details
    /// </summary>
    public ErrorResponse Error { get; set; }

    /// <summary>
    /// Timestamp when response was generated (UTC)
    /// </summary>
    public DateTime Timestamp { get; set; }

    public StandardizedErrorResponse(string requestId, ErrorResponse error)
    {
        RequestId = requestId;
        Error = error;
        Timestamp = DateTime.UtcNow;
    }
}
