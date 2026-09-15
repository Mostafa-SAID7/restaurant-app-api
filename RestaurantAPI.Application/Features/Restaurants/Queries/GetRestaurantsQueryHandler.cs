using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

/// <summary>
/// Handler for GetRestaurantsQuery.
/// Retrieves all restaurants with optional filtering by category, address, or name.
/// </summary>
public class GetRestaurantsQueryHandler : IRequestHandler<GetRestaurantsQuery, IEnumerable<RestaurantDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRestaurantsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RestaurantDto>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
    {
        // Get restaurants with optional filters
        var restaurants = await _unitOfWork.Restaurants.GetByFiltersAsync(
            request.Category ?? "",
            request.Address,
            request.Name
        );

        // Map to DTOs
        return _mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
    }
}
