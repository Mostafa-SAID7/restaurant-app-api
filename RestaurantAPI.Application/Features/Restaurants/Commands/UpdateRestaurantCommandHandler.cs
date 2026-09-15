using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Restaurants.Commands;

/// <summary>
/// Handler for UpdateRestaurantCommand.
/// Updates an existing restaurant with new data.
/// </summary>
public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, RestaurantDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRestaurantCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RestaurantDto> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (request.RestaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be greater than 0");

        if (request.RestaurantData == null)
            throw new ArgumentNullException(nameof(request.RestaurantData), "Restaurant data is required");

        // Get existing restaurant
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant with ID {request.RestaurantId} not found");

        // Update properties
        restaurant.RestaurantName = request.RestaurantData.RestaurantName;
        restaurant.Address = request.RestaurantData.Address;
        restaurant.Type = request.RestaurantData.Type;
        restaurant.ParkingLot = request.RestaurantData.ParkingLot;
        restaurant.UpdatedAt = DateTime.UtcNow;

        // Persist
        await _unitOfWork.Restaurants.UpdateAsync(restaurant);
        await _unitOfWork.SaveChangesAsync();

        // Return as DTO
        return _mapper.Map<RestaurantDto>(restaurant);
    }
}
