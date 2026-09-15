using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.DTOs;

public class ApiResponseWrapper<T>
{
    public int StatusCode { get; set; }
    public ApiResponse<T>? Response { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public long ExecutionTimeMs { get; set; }
    public string? RequestId { get; set; }

    public static ApiResponseWrapper<T> CreateSuccess(T data, string message = "Operation completed successfully") => new()
    {
        StatusCode = 200,
        Response = ApiResponse<T>.CreateSuccess(data, message),
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponseWrapper<T> CreateCreated(T data, string message = "Resource created successfully") => new()
    {
        StatusCode = 201,
        Response = ApiResponse<T>.CreateSuccess(data, message),
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponseWrapper<T> CreateError(string message, int statusCode = 400, object? details = null) => new()
    {
        StatusCode = statusCode,
        Response = ApiResponse<T>.CreateError(message, details),
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponseWrapper<T> CreateValidationError(List<string> errors) => new()
    {
        StatusCode = 422,
        Response = ApiResponse<T>.CreateValidationError(errors),
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponseWrapper<T> CreateNotFound(string message = "Resource not found") => new()
    {
        StatusCode = 404,
        Response = ApiResponse<T>.CreateError(message),
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponseWrapper<T> CreateUnauthorized(string message = "Unauthorized access") => new()
    {
        StatusCode = 401,
        Response = ApiResponse<T>.CreateError(message),
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponseWrapper<T> CreateForbidden(string message = "Access forbidden") => new()
    {
        StatusCode = 403,
        Response = ApiResponse<T>.CreateError(message),
        Timestamp = DateTime.UtcNow
    };
}
