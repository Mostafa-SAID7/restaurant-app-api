using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Restaurants.Commands;

/// <summary>
/// Handler for CreateRestaurantCommand.
/// Creates a new restaurant in the system.
/// 
/// Business Logic:
/// 1. Map input DTO to Restaurant entity
/// 2. Persist to database
/// 3. Return created restaurant as DTO
/// </summary>
public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, RestaurantDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RestaurantDto> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (request.RestaurantData == null)
            throw new ArgumentNullException(nameof(request.RestaurantData), "Restaurant data is required");

        if (string.IsNullOrWhiteSpace(request.RestaurantData.RestaurantName))
            throw new ArgumentException("Restaurant name is required");

        if (string.IsNullOrWhiteSpace(request.RestaurantData.Address))
            throw new ArgumentException("Restaurant address is required");

        if (string.IsNullOrWhiteSpace(request.RestaurantData.Type))
            throw new ArgumentException("Restaurant type is required");

        // Map DTO to entity
        var restaurant = _mapper.Map<Restaurant>(request.RestaurantData);

        // Persist
        await _unitOfWork.Restaurants.AddAsync(restaurant);
        await _unitOfWork.SaveChangesAsync();

        // Return as DTO
        return _mapper.Map<RestaurantDto>(restaurant);
    }
}
