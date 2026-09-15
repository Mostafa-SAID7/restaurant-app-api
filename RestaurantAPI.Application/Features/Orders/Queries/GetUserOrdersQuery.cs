using MediatR;

namespace RestaurantAPI.Application.Features.Orders.Queries;

/// <summary>
/// Query to get all orders for a user.
/// </summary>
public class GetUserOrdersQuery : IRequest<GetUserOrdersResponse>
{
    public string UserId { get; set; } = null!;
}

public class GetUserOrdersResponse
{
    public int MasterID { get; set; }
    public string? UserCode { get; set; }
    public int RestaurantID { get; set; }
    public string? RestaurantName { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
}

