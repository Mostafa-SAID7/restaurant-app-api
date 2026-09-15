using MediatR;
using RestaurantAPI.Application.Common.DTOs;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

public class GetAllRestaurantsQuery : IRequest<List<RestaurantDto>>
{
}
