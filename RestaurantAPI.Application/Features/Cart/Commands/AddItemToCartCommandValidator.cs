using FluentValidation;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Validator for AddItemToCartCommand.
/// Centralizes input validation using FluentValidation.
/// </summary>
public class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.CartItem)
            .NotNull()
            .WithMessage("Cart item data is required");

        RuleFor(x => x.CartItem.ItemID)
            .GreaterThan(0)
            .WithMessage("Item ID must be greater than 0");

        RuleFor(x => x.CartItem.Quantity)
            .InclusiveBetween(1, 100)
            .WithMessage("Quantity must be between 1 and 100");
    }
}
