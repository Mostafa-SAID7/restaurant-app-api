using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Orders.Commands;

/// <summary>
/// Command to create an order from order line items.
/// Handles business logic: validation, transaction, state snapshots.
/// </summary>
public class CreateOrderCommand : IRequest<CreateOrderResponseDto>
{
    public int RestaurantId { get; set; }
    public string UserId { get; set; } = null!;
    public CreateOrderDto OrderData { get; set; } = new();
}
