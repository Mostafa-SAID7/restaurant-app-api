namespace RestaurantAPI.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-specific business logic errors.
/// Use this when a business rule is violated (not for infrastructure/EF errors).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
