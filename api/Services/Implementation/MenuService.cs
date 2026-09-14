using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Services.Implementation;

/// <summary>
/// Menu service for restaurant menu operations
/// Phase A.3: Extracted from RestaurantService for SRP
/// </summary>
public class MenuService : IMenuService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MenuService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get menu items for a specific restaurant
    /// </summary>
    public async Task<IEnumerable<ItemResponseDTO>> GetMenuAsync(int restaurantId, string sortByPrice = "")
    {
        var items = await _unitOfWork.Items.GetMenuByRestaurantIdAsync(restaurantId, sortByPrice);
        return _mapper.Map<IEnumerable<ItemResponseDTO>>(items);
    }
}
