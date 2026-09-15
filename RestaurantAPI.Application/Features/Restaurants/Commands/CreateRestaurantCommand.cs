using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Commands;

/// <summary>
/// Command to create a new restaurant.
/// Maps to: RestaurantService.CreateRestaurantAsync(restaurantDTO)
/// </summary>
public class CreateRestaurantCommand : IRequest<RestaurantDTO>
{
    /// <summary>
    /// Restaurant data for creation (name, address, type, parking lot flag).
    /// </summary>
    public RestaurantDTO RestaurantData { get; set; } = null!;
}
