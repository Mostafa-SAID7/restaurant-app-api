using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

/// <summary>
/// Query to get menu items for a specific restaurant.
/// Maps to: MenuService.GetMenuAsync(restaurantId, sortByPrice)
/// </summary>
public class GetRestaurantMenuQuery : IRequest<IEnumerable<ItemResponseDto>>
{
    /// <summary>
    /// Restaurant ID whose menu to retrieve.
    /// </summary>
    public int RestaurantId { get; set; }

    /// <summary>
    /// Optional sort parameter (e.g., "price_asc", "price_desc").
    /// </summary>
    public string? SortByPrice { get; set; }
}
