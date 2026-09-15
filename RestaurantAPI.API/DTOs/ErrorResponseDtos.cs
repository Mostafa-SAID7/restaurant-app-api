namespace RestaurantAPI.DTOs;

public enum ErrorCode
{
    InvalidRequest,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    RateLimitExceeded,
    BusinessRuleViolation,
    InternalError,
    ValidationFailed
}

public class ValidationError
{
    public string Field { get; set; }
    public string Message { get; set; }

    public ValidationError(string field, string message)
    {
        Field = field;
        Message = message;
    }
}

public class ErrorResponse
{
    public ErrorCode ErrorCode { get; set; }
    public int StatusCode { get; set; }
    public string ErrorType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ErrorResponse ValidationFailed(List<ValidationError> errors) => new()
    {
        ErrorCode = ErrorCode.ValidationFailed,
        StatusCode = 422,
        ErrorType = "VALIDATION_FAILED",
        Message = "Validation failed",
        Details = errors,
        Timestamp = DateTime.UtcNow
    };

    public static ErrorResponse ResourceNotFound(string resourceType, object? identifier = null) => new()
    {
        ErrorCode = ErrorCode.NotFound,
        StatusCode = 404,
        ErrorType = "NOT_FOUND",
        Message = identifier != null
            ? $"{resourceType} with identifier '{identifier}' was not found"
            : $"{resourceType} not found",
        Timestamp = DateTime.UtcNow
    };

    public static ErrorResponse Unauthorized(string message = "Unauthorized access") => new()
    {
        ErrorCode = ErrorCode.Unauthorized,
        StatusCode = 401,
        ErrorType = "UNAUTHORIZED",
        Message = message,
        Timestamp = DateTime.UtcNow
    };

    public static ErrorResponse Forbidden(string message = "Access forbidden") => new()
    {
        ErrorCode = ErrorCode.Forbidden,
        StatusCode = 403,
        ErrorType = "FORBIDDEN",
        Message = message,
        Timestamp = DateTime.UtcNow
    };

    public static ErrorResponse Conflict(string message, object? details = null) => new()
    {
        ErrorCode = ErrorCode.Conflict,
        StatusCode = 409,
        ErrorType = "CONFLICT",
        Message = message,
        Details = details,
        Timestamp = DateTime.UtcNow
    };

    public static ErrorResponse RateLimitExceeded(int retryAfterSeconds) => new()
    {
        ErrorCode = ErrorCode.RateLimitExceeded,
        StatusCode = 429,
        ErrorType = "RATE_LIMIT_EXCEEDED",
        Message = "Too many requests. Please try again later.",
        Details = new { retryAfterSeconds },
        Timestamp = DateTime.UtcNow
    };

    public static ErrorResponse BusinessRuleViolation(string message, object? details = null) => new()
    {
        ErrorCode = ErrorCode.BusinessRuleViolation,
        StatusCode = 422,
        ErrorType = "BUSINESS_RULE_VIOLATION",
        Message = message,
        Details = details,
        Timestamp = DateTime.UtcNow
    };

    public static ErrorResponse InternalError(string message = "An unexpected error occurred", string? stackTrace = null) => new()
    {
        ErrorCode = ErrorCode.InternalError,
        StatusCode = 500,
        ErrorType = "INTERNAL_ERROR",
        Message = message,
        StackTrace = stackTrace,
        Timestamp = DateTime.UtcNow
    };
}

public class StandardizedErrorResponse
{
    public string RequestId { get; set; }
    public ErrorResponse Error { get; set; }

    public StandardizedErrorResponse(string requestId, ErrorResponse error)
    {
        RequestId = requestId;
        Error = error;
    }
}
