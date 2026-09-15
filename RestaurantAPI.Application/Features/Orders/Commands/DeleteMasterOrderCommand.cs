using MediatR;

namespace RestaurantAPI.Application.Features.Orders.Commands;

/// <summary>
/// Command to delete a complete master order and all its line items.
/// </summary>
public class DeleteMasterOrderCommand : IRequest<bool>
{
    public int MasterId { get; set; }
    public string UserId { get; set; } = null!;
}
