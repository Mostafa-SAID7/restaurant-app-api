using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Commands;

/// <summary>
/// Command to create a new restaurant.
/// Maps to: RestaurantService.CreateRestaurantAsync(restaurantDto)
/// </summary>
public class CreateRestaurantCommand : IRequest<RestaurantDto>
{
    /// <summary>
    /// Restaurant data for creation (name, address, type, parking lot flag).
    /// </summary>
    public CreateRestaurantDto RestaurantData { get; set; } = null!;
}
