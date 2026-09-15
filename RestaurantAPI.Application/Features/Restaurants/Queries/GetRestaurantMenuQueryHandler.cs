using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

/// <summary>
/// Handler for GetRestaurantMenuQuery.
/// Retrieves all menu items for a specific restaurant with optional sorting.
/// </summary>
public class GetRestaurantMenuQueryHandler : IRequestHandler<GetRestaurantMenuQuery, IEnumerable<ItemResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRestaurantMenuQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemResponseDTO>> Handle(GetRestaurantMenuQuery request, CancellationToken cancellationToken)
    {
        if (request.RestaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be greater than 0");

        // Verify restaurant exists
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant with ID {request.RestaurantId} not found");

        // Get menu items for the restaurant
        var items = await _unitOfWork.Items.GetMenuByRestaurantIdAsync(
            request.RestaurantId,
            request.SortByPrice ?? ""
        );

        return _mapper.Map<IEnumerable<ItemResponseDTO>>(items);
    }
}
