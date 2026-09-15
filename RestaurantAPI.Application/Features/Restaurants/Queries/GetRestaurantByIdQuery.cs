using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

/// <summary>
/// Query to get a restaurant by its ID.
/// Maps to: RestaurantService.GetRestaurantByIdAsync(restaurantId)
/// </summary>
public class GetRestaurantByIdQuery : IRequest<RestaurantDto?>
{
    /// <summary>
    /// Restaurant ID to retrieve.
    /// </summary>
    public int RestaurantId { get; set; }
}
