using FluentValidation;

namespace RestaurantAPI.Application.Features.Orders.Commands;

/// <summary>
/// Validator for CreateOrderCommand.
/// Centralizes order creation input validation using FluentValidation.
/// This validator runs via MediatR pipeline before handler is called.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .GreaterThan(0)
            .WithMessage("Restaurant ID must be greater than 0");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.OrderData)
            .NotNull()
            .WithMessage("Order data is required");

        RuleFor(x => x.OrderData.Items)
            .NotEmpty()
            .WithMessage("At least one item must be ordered");

        RuleForEach(x => x.OrderData.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Quantity)
                    .InclusiveBetween(1, 100)
                    .WithMessage("Quantity must be between 1 and 100");

                item.RuleFor(x => x)
                    .Must(x => x.ItemID.HasValue && x.ItemID > 0 || !string.IsNullOrWhiteSpace(x.ItemName))
                    .WithMessage("Either ItemID or ItemName must be provided");
            });
    }
}
