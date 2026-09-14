using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Services.Implementation;

/// <summary>
/// Restaurant service for restaurant management operations
/// Phase A.5: Returns DTOs only, never entities (SRP, DIP)
/// </summary>
public class RestaurantService : IRestaurantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RestaurantService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RestaurantDTO>> GetRestaurantsAsync(string category = "", string? address = null, string? name = null)
    {
        var restaurants = await _unitOfWork.Restaurants.GetByFiltersAsync(category, address, name);
        return _mapper.Map<IEnumerable<RestaurantDTO>>(restaurants);
    }

    public async Task<RestaurantDTO?> GetRestaurantByIdAsync(int restaurantId)
    {
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
        return restaurant == null ? null : _mapper.Map<RestaurantDTO>(restaurant);
    }

    public async Task<RestaurantDTO> CreateRestaurantAsync(RestaurantDTO restaurantDTO)
    {
        var restaurant = _mapper.Map<Restaurant>(restaurantDTO);
        
        await _unitOfWork.Restaurants.AddAsync(restaurant);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<RestaurantDTO>(restaurant);
    }

    public async Task<bool> RestaurantExistsAsync(string restaurantName)
    {
        return await _unitOfWork.Restaurants.RestaurantExistsByNameAsync(restaurantName);
    }

    public async Task UpdateImageUrlsAsync(int restaurantId, string[] urls)
    {
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant with ID {restaurantId} not found");

        // This method can be implemented based on your specific requirements
        // For now, it's a placeholder implementation
        await Task.CompletedTask;
    }
}