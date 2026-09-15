using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Orders.Queries;

/// <summary>
/// Query to get order lines for a specific master order.
/// </summary>
public class GetOrderByMasterIdQuery : IRequest<List<OrderLineDto>>
{
    public string UserId { get; set; } = null!;
    public int MasterId { get; set; }
}
