using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;

namespace RestaurantAPI.Helpers;

/// <summary>
/// Response helper for standardized API responses
/// Phase A.4: Uses unified ApiResponse<T> contract
/// Phase B.2: Extended with ApiResponseWrapper for request tracking
/// </summary>
public static class ResponseHelper
{
    /// <summary>
    /// Creates a standardized success response
    /// </summary>
    public static ActionResult Success<T>(T data, string? message = null)
    {
        var response = ApiResponse<T>.CreateSuccess(data, message ?? "Operation completed successfully");
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Creates a wrapped success response with request tracking
    /// </summary>
    public static ActionResult SuccessWrapped<T>(T data, string? message = null)
    {
        var wrapper = ApiResponseWrapper<T>.CreateSuccess(data, message ?? "Operation completed successfully");
        return new OkObjectResult(wrapper);
    }

    /// <summary>
    /// Creates a standardized error response
    /// </summary>
    public static ActionResult Error(string message, int statusCode = 400, object? details = null)
    {
        var response = ApiResponse<object>.CreateError(message, details);
        return new ObjectResult(response) { StatusCode = statusCode };
    }

    /// <summary>
    /// Creates a wrapped error response with request tracking
    /// </summary>
    public static ActionResult ErrorWrapped(string message, int statusCode = 400, object? details = null)
    {
        var wrapper = ApiResponseWrapper<object>.CreateError(message, statusCode, details);
        return new ObjectResult(wrapper) { StatusCode = statusCode };
    }

    /// <summary>
    /// Creates a validation error response
    /// </summary>
    public static ActionResult ValidationError(List<string> errors)
    {
        var response = ApiResponse<object>.CreateValidationError(errors);
        return new BadRequestObjectResult(response);
    }

    /// <summary>
    /// Creates a wrapped validation error response
    /// </summary>
    public static ActionResult ValidationErrorWrapped(List<string> errors)
    {
        var wrapper = ApiResponseWrapper<object>.CreateValidationError(errors);
        return new ObjectResult(wrapper) { StatusCode = 422 };
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    public static ActionResult NotFound(string resource, object? identifier = null)
    {
        var message = identifier != null 
            ? $"{resource} with identifier '{identifier}' was not found"
            : $"{resource} not found";

        var response = ApiResponse<object>.CreateError(message);
        return new NotFoundObjectResult(response);
    }

    /// <summary>
    /// Creates a wrapped not found response
    /// </summary>
    public static ActionResult NotFoundWrapped(string message = "Resource not found")
    {
        var wrapper = ApiResponseWrapper<object>.CreateNotFound(message);
        return new ObjectResult(wrapper) { StatusCode = 404 };
    }

    /// <summary>
    /// Creates an unauthorized response
    /// </summary>
    public static ActionResult Unauthorized(string message = "Unauthorized access")
    {
        var response = ApiResponse<object>.CreateError(message);
        return new UnauthorizedObjectResult(response);
    }

    /// <summary>
    /// Creates a wrapped unauthorized response
    /// </summary>
    public static ActionResult UnauthorizedWrapped(string message = "Unauthorized access")
    {
        var wrapper = ApiResponseWrapper<object>.CreateUnauthorized(message);
        return new ObjectResult(wrapper) { StatusCode = 401 };
    }

    /// <summary>
    /// Creates a forbidden response
    /// </summary>
    public static ActionResult Forbidden(string message = "Access forbidden")
    {
        var wrapper = ApiResponseWrapper<object>.CreateForbidden(message);
        return new ObjectResult(wrapper) { StatusCode = 403 };
    }

    /// <summary>
    /// Creates a paginated response
    /// </summary>
    public static ActionResult Paginated<T>(IEnumerable<T> data, int page, int pageSize, int totalCount)
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        var response = new ApiResponse<IEnumerable<T>>
        {
            Success = true,
            Data = data,
            Message = "Operation completed successfully",
            Pagination = new PaginationMetadata
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
    /// Creates a standardized paginated response (Phase B.4)
    /// </summary>
    public static ActionResult PaginatedStandard<T>(PaginatedResponse<T> paginatedData)
    {
        var response = new ApiResponse<List<T>>
        {
            Success = true,
            Data = paginatedData.Data,
            Message = "Operation completed successfully",
            Pagination = new PaginationMetadata
            {
                Page = paginatedData.Pagination.PageNumber,
                PageSize = paginatedData.Pagination.PageSize,
                TotalCount = paginatedData.Pagination.TotalCount,
                TotalPages = paginatedData.Pagination.TotalPages,
                HasNextPage = paginatedData.Pagination.HasNextPage,
                HasPreviousPage = paginatedData.Pagination.HasPreviousPage
            },
            Timestamp = DateTime.UtcNow
        };

        return new OkObjectResult(response);
    }

    /// <summary>
    /// Creates a wrapped paginated response with request tracking (Phase B.4)
    /// </summary>
    public static ActionResult PaginatedWrapped<T>(PaginatedResponse<T> paginatedData)
    {
        var wrapper = new ApiResponseWrapper<List<T>>
        {
            StatusCode = 200,
            Response = new ApiResponse<List<T>>
            {
                Success = true,
                Data = paginatedData.Data,
                Message = "Operation completed successfully",
                Pagination = new PaginationMetadata
                {
                    Page = paginatedData.Pagination.PageNumber,
                    PageSize = paginatedData.Pagination.PageSize,
                    TotalCount = paginatedData.Pagination.TotalCount,
                    TotalPages = paginatedData.Pagination.TotalPages,
                    HasNextPage = paginatedData.Pagination.HasNextPage,
                    HasPreviousPage = paginatedData.Pagination.HasPreviousPage
                },
                Timestamp = DateTime.UtcNow
            },
            Timestamp = DateTime.UtcNow
        };

        return new OkObjectResult(wrapper);
    }

    /// <summary>
    /// Creates a created response (201)
    /// </summary>
    public static ActionResult Created<T>(T data, string? location = null)
    {
        var response = ApiResponse<T>.CreateSuccess(data, "Resource created successfully");
        var result = new ObjectResult(response) { StatusCode = 201 };
        
        return result;
    }

    /// <summary>
    /// Creates a wrapped created response (201)
    /// </summary>
    public static ActionResult CreatedWrapped<T>(T data, string? message = null)
    {
        var wrapper = ApiResponseWrapper<T>.CreateCreated(data, message ?? "Resource created successfully");
        return new ObjectResult(wrapper) { StatusCode = 201 };
    }
}