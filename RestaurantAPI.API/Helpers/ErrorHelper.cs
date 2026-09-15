using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;

namespace RestaurantAPI.Helpers;

/// <summary>
/// Helper for standardized error responses
/// Phase B.5: Consistent error handling across all endpoints
/// </summary>
public static class ErrorHelper
{
    /// <summary>
    /// Creates a standardized validation error response
    /// </summary>
    public static ActionResult ValidationError(List<ValidationError> errors)
    {
        var errorResponse = ErrorResponse.ValidationFailed(errors);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 422 
        };
    }

    /// <summary>
    /// Creates a standardized validation error response from error messages
    /// </summary>
    public static ActionResult ValidationError(List<string> errorMessages)
    {
        var validationErrors = errorMessages
            .Select((msg, idx) => new ValidationError($"field_{idx}", msg))
            .ToList();

        return ValidationError(validationErrors);
    }

    /// <summary>
    /// Creates a standardized not found error response
    /// </summary>
    public static ActionResult NotFound(string resourceType, object? identifier = null)
    {
        var errorResponse = ErrorResponse.ResourceNotFound(resourceType, identifier);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 404 
        };
    }

    /// <summary>
    /// Creates a standardized unauthorized error response
    /// </summary>
    public static ActionResult Unauthorized(string message = "Unauthorized access")
    {
        var errorResponse = ErrorResponse.Unauthorized(message);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 401 
        };
    }

    /// <summary>
    /// Creates a standardized forbidden error response
    /// </summary>
    public static ActionResult Forbidden(string message = "Access forbidden")
    {
        var errorResponse = ErrorResponse.Forbidden(message);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 403 
        };
    }

    /// <summary>
    /// Creates a standardized conflict error response
    /// </summary>
    public static ActionResult Conflict(string message, object? details = null)
    {
        var errorResponse = ErrorResponse.Conflict(message, details);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 409 
        };
    }

    /// <summary>
    /// Creates a standardized rate limit error response
    /// </summary>
    public static ActionResult RateLimitExceeded(int retryAfterSeconds)
    {
        var errorResponse = ErrorResponse.RateLimitExceeded(retryAfterSeconds);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 429 
        };
    }

    /// <summary>
    /// Creates a standardized business rule violation error response
    /// </summary>
    public static ActionResult BusinessRuleViolation(string message, object? details = null)
    {
        var errorResponse = ErrorResponse.BusinessRuleViolation(message, details);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 422 
        };
    }

    /// <summary>
    /// Creates a standardized internal server error response
    /// </summary>
    public static ActionResult InternalError(string message = "An unexpected error occurred", string? stackTrace = null)
    {
        var errorResponse = ErrorResponse.InternalError(message, stackTrace);
        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 500 
        };
    }

    /// <summary>
    /// Creates a standardized bad request error response
    /// </summary>
    public static ActionResult BadRequest(string message, object? details = null)
    {
        var errorResponse = new ErrorResponse
        {
            ErrorCode = ErrorCode.InvalidRequest,
            StatusCode = 400,
            ErrorType = "INVALID_REQUEST",
            Message = message,
            Details = details,
            Timestamp = DateTime.UtcNow
        };

        var standardized = new StandardizedErrorResponse(Guid.NewGuid().ToString("N")[..12], errorResponse);
        
        return new ObjectResult(standardized) 
        { 
            StatusCode = 400 
        };
    }
}
