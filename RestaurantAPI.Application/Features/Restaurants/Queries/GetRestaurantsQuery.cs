using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

/// <summary>
/// Query to get all restaurants with optional filtering.
/// Maps to: RestaurantService.GetRestaurantsAsync(category, address, name)
/// </summary>
public class GetRestaurantsQuery : IRequest<IEnumerable<RestaurantDto>>
{
    /// <summary>
    /// Filter by restaurant category/type (e.g., "Italian", "Fast Food").
    /// Optional.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Filter by address.
    /// Optional.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Filter by restaurant name.
    /// Optional.
    /// </summary>
    public string? Name { get; set; }
}
