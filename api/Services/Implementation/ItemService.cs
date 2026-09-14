using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Services.Implementation;

/// <summary>
/// Item service for menu item operations
/// Phase A.5: Returns DTOs only, never entities
/// </summary>
public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Add a new item to a restaurant menu
    /// </summary>
    public async Task<ItemDTO> AddItemToMenuAsync(int restaurantId, ItemDTO itemDTO)
    {
        var item = _mapper.Map<Item>(itemDTO);
        item.RestaurantID = restaurantId;
        
        await _unitOfWork.Items.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<ItemDTO>(item);
    }

    /// <summary>
    /// Get all menu items (across all restaurants) with optional filtering
    /// </summary>
    public async Task<IEnumerable<ItemResponseDTO>> GetAllItemsAsync(string itemName = "", string sortByPrice = "")
    {
        var items = await _unitOfWork.Items.GetByFiltersAsync(itemName, sortByPrice);
        return _mapper.Map<IEnumerable<ItemResponseDTO>>(items);
    }
}

