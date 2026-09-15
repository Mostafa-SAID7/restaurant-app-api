using MediatR;

namespace RestaurantAPI.Application.Features.Orders.Commands;

/// <summary>
/// Command to delete a single order line.
/// </summary>
public class DeleteOrderCommand : IRequest<bool>
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = null!;
}
