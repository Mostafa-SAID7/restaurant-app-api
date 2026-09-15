using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

/// <summary>
/// Query to get all menu items across all restaurants with optional filtering.
/// Maps to: ItemService.GetAllItemsAsync(itemName, sortByPrice)
/// </summary>
public class GetAllItemsQuery : IRequest<IEnumerable<ItemResponseDto>>
{
    /// <summary>
    /// Filter by item name (partial match).
    /// Optional.
    /// </summary>
    public string? ItemName { get; set; }

    /// <summary>
    /// Optional sort parameter (e.g., "price_asc", "price_desc").
    /// </summary>
    public string? SortByPrice { get; set; }
}
