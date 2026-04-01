using AutoMapper;
using FakeRestuarantAPI.Models;
using FakeRestuarantAPI.Repositories.Interfaces;
using FakeRestuarantAPI.Services.Interfaces;

namespace FakeRestuarantAPI.Services.Implementation;

public class RestaurantService : IRestaurantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RestaurantService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsAsync(string category = "", string? address = null, string? name = null)
    {
        return await _unitOfWork.Restaurants.GetByFiltersAsync(category, address, name);
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId)
    {
        return await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
    }

    public async Task<Restaurant> CreateRestaurantAsync(RestaurantDTO restaurantDTO)
    {
        var restaurant = _mapper.Map<Restaurant>(restaurantDTO);
        
        await _unitOfWork.Restaurants.AddAsync(restaurant);
        await _unitOfWork.SaveChangesAsync();
        
        return restaurant;
    }

    public async Task<bool> RestaurantExistsAsync(string restaurantName)
    {
        return await _unitOfWork.Restaurants.RestaurantExistsByNameAsync(restaurantName);
    }

    public async Task<IEnumerable<GetItems>> GetMenuAsync(int restaurantId, string sortByPrice = "")
    {
        var items = await _unitOfWork.Items.GetMenuByRestaurantIdAsync(restaurantId, sortByPrice);
        return _mapper.Map<IEnumerable<GetItems>>(items);
    }

    public async Task<Item> AddItemToMenuAsync(int restaurantId, ItemDTO itemDTO)
    {
        var item = _mapper.Map<Item>(itemDTO);
        item.RestaurantID = restaurantId;
        
        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        
        return item;
    }

    public async Task<IEnumerable<GetItems>> GetAllItemsAsync(string itemName = "", string sortByPrice = "")
    {
        var items = await _unitOfWork.Items.GetByFiltersAsync(itemName, sortByPrice);
        return _mapper.Map<IEnumerable<GetItems>>(items);
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