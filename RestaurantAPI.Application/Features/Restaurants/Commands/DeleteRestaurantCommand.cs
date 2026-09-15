using MediatR;

namespace RestaurantAPI.Application.Features.Restaurants.Commands;

/// <summary>
/// Command to delete a restaurant.
/// </summary>
public class DeleteRestaurantCommand : IRequest<bool>
{
    /// <summary>
    /// Restaurant ID to delete.
    /// </summary>
    public int RestaurantId { get; set; }
}
