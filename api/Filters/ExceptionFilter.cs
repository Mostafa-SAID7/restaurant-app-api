using RestuarantAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace RestuarantAPI.Filters;

/// <summary>
/// Global exception filter to handle unhandled exceptions and return standardized error responses
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        
        // Log the exception
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var response = CreateErrorResponse(exception);
        
        context.Result = new ObjectResult(response.Data)
        {
            StatusCode = response.StatusCode
        };

        context.ExceptionHandled = true;
    }

    private (object Data, int StatusCode) CreateErrorResponse(Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            NotImplementedException => (int)HttpStatusCode.NotImplemented,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            Success = false,
            Message = GetUserFriendlyMessage(exception),
            Details = _environment.IsDevelopment() ? exception.ToString() : null,
            Timestamp = DateTime.UtcNow
        };

        return (response, statusCode);
    }

    private string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => "Required parameter is missing",
            ArgumentException => "Invalid request parameters",
            UnauthorizedAccessException => "Access denied",
            KeyNotFoundException => "Requested resource not found",
            NotImplementedException => "Feature not implemented",
            TimeoutException => "Request timeout",
            _ => "An error occurred while processing your request"
        };
    }
}