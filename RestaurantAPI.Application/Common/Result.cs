namespace RestaurantAPI.Application.Common;

/// <summary>
/// Result type for handler responses - supports both success and error states.
/// Eliminates the need for throwing exceptions for expected failures.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> ValidationErrors { get; set; } = new();

    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static Result<T> ValidationFailure(List<string> errors) => new() 
    { 
        IsSuccess = false, 
        ErrorMessage = "Validation failed", 
        ValidationErrors = errors 
    };
}

/// <summary>
/// Non-generic Result for void operations.
/// </summary>
public class Result
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> ValidationErrors { get; set; } = new();

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static Result ValidationFailure(List<string> errors) => new()
    {
        IsSuccess = false,
        ErrorMessage = "Validation failed",
        ValidationErrors = errors
    };
}
