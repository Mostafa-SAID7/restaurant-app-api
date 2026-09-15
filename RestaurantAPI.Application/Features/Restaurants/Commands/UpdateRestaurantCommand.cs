using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Commands;

/// <summary>
/// Command to update an existing restaurant.
/// </summary>
public class UpdateRestaurantCommand : IRequest<RestaurantDto>
{
    /// <summary>
    /// Restaurant ID to update.
    /// </summary>
    public int RestaurantId { get; set; }

    /// <summary>
    /// Updated restaurant data.
    /// </summary>
    public CreateRestaurantDto RestaurantData { get; set; } = null!;
}
