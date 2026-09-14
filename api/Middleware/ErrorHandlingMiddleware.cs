using System.Text.Json;
using RestaurantAPI.DTOs;

namespace RestaurantAPI.Middleware;

/// <summary>
/// Global error handling middleware
/// Phase B.5: Standardizes error responses across all endpoints
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly bool _includeSensitiveDetails;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _includeSensitiveDetails = environment.IsDevelopment();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString("N")[..12];

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in request {RequestId}", requestId);
            await HandleExceptionAsync(context, ex, requestId);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, string requestId)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            ArgumentException => CreateArgumentError(exception, requestId),
            UnauthorizedAccessException => CreateUnauthorizedError(exception, requestId),
            KeyNotFoundException => CreateNotFoundError(exception, requestId),
            InvalidOperationException => CreateBusinessRuleError(exception, requestId),
            _ => CreateInternalError(exception, requestId)
        };

        context.Response.StatusCode = errorResponse.Error.StatusCode;

        var jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };

        return context.Response.WriteAsJsonAsync(errorResponse, jsonOptions);
    }

    private static StandardizedErrorResponse CreateArgumentError(Exception exception, string requestId)
    {
        var error = new ErrorResponse
        {
            ErrorCode = ErrorCode.InvalidRequest,
            StatusCode = 400,
            ErrorType = "INVALID_REQUEST",
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        return new StandardizedErrorResponse(requestId, error);
    }

    private static StandardizedErrorResponse CreateUnauthorizedError(Exception exception, string requestId)
    {
        var error = new ErrorResponse
        {
            ErrorCode = ErrorCode.Unauthorized,
            StatusCode = 401,
            ErrorType = "UNAUTHORIZED",
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        return new StandardizedErrorResponse(requestId, error);
    }

    private static StandardizedErrorResponse CreateNotFoundError(Exception exception, string requestId)
    {
        var error = new ErrorResponse
        {
            ErrorCode = ErrorCode.NotFound,
            StatusCode = 404,
            ErrorType = "NOT_FOUND",
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        return new StandardizedErrorResponse(requestId, error);
    }

    private static StandardizedErrorResponse CreateBusinessRuleError(Exception exception, string requestId)
    {
        var error = new ErrorResponse
        {
            ErrorCode = ErrorCode.BusinessRuleViolation,
            StatusCode = 422,
            ErrorType = "BUSINESS_RULE_VIOLATION",
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        return new StandardizedErrorResponse(requestId, error);
    }

    private static StandardizedErrorResponse CreateInternalError(Exception exception, string requestId)
    {
        var error = new ErrorResponse
        {
            ErrorCode = ErrorCode.InternalError,
            StatusCode = 500,
            ErrorType = "INTERNAL_ERROR",
            Message = "An unexpected error occurred",
            StackTrace = exception.StackTrace,
            Timestamp = DateTime.UtcNow
        };

        return new StandardizedErrorResponse(requestId, error);
    }
}

/// <summary>
/// Extension method to add error handling middleware
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
