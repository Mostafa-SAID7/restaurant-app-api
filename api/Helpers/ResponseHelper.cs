using Microsoft.AspNetCore.Mvc;

namespace RestuarantAPI.Helpers;

public static class ResponseHelper
{
    /// <summary>
    /// Creates a standardized success response
    /// </summary>
    public static ActionResult Success<T>(T data, string? message = null)
    {
        var response = new
        {
            Success = true,
            Message = message ?? "Operation completed successfully",
            Data = data,
            Timestamp = DateTime.UtcNow
        };

        return new OkObjectResult(response);
    }

    /// <summary>
    /// Creates a standardized error response
    /// </summary>
    public static ActionResult Error(string message, int statusCode = 400, object? details = null)
    {
        var response = new
        {
            Success = false,
            Message = message,
            Details = details,
            Timestamp = DateTime.UtcNow
        };

        return new ObjectResult(response) { StatusCode = statusCode };
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    public static ActionResult ValidationError(List<string> errors)
    {
        var response = new
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };

        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    public static ActionResult NotFound(string resource, object? identifier = null)
    {
        var message = identifier != null 
            ? $"{resource} with identifier '{identifier}' was not found"
            : $"{resource} not found";

        var response = new
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        return new NotFoundObjectResult(response);
    }

    /// <summary>
    /// Creates an unauthorized response
    /// </summary>
    public static ActionResult Unauthorized(string message = "Unauthorized access")
    {
        var response = new
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        return new UnauthorizedObjectResult(response);
    }

    /// <summary>
    /// Creates a paginated response
    /// </summary>
    public static ActionResult Paginated<T>(IEnumerable<T> data, int page, int pageSize, int totalCount)
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        var response = new
        {
            Success = true,
            Data = data,
            Pagination = new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            },
            Timestamp = DateTime.UtcNow
        };

        return new OkObjectResult(response);
    }

    /// <summary>
    /// Creates a created response
    /// </summary>
    public static ActionResult Created<T>(T data, string? location = null)
    {
        var response = new
        {
            Success = true,
            Message = "Resource created successfully",
            Data = data,
            Timestamp = DateTime.UtcNow
        };

        var result = new ObjectResult(response) { StatusCode = 201 };
        
        if (!string.IsNullOrEmpty(location))
        {
            result.Value = response;
        }

        return result;
    }
}