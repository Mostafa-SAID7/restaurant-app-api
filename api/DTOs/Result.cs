namespace RestaurantAPI.DTOs;

/// <summary>
/// Result wrapper for service operations
/// Phase A.5: All services return Result<T>, not raw types
/// Allows for consistent error handling and DTO returns
/// </summary>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation succeeded
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Operation result data (null if failed)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error message (null if succeeded)
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Success result factory
    /// </summary>
    public static Result<T> Ok(T data)
    {
        return new Result<T>
        {
            Success = true,
            Data = data
        };
    }

    /// <summary>
    /// Success result factory with message
    /// </summary>
    public static Result<T> Ok(T data, string message)
    {
        return new Result<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Failure result factory
    /// </summary>
    public static Result<T> Fail(string message)
    {
        return new Result<T>
        {
            Success = false,
            Message = message
        };
    }
}

/// <summary>
/// Non-generic result for operations with no return value
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation succeeded
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message (null if succeeded)
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Success result factory
    /// </summary>
    public static Result Ok()
    {
        return new Result { Success = true };
    }

    /// <summary>
    /// Success result factory with message
    /// </summary>
    public static Result Ok(string message)
    {
        return new Result { Success = true, Message = message };
    }

    /// <summary>
    /// Failure result factory
    /// </summary>
    public static Result Fail(string message)
    {
        return new Result { Success = false, Message = message };
    }
}
