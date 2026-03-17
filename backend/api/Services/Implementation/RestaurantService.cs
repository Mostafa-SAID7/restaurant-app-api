using AutoMapper;
using FakeRestuarantAPI.Data;
using FakeRestuarantAPI.Models;
using FakeRestuarantAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FakeRestuarantAPI.Services.Implementation;

public class RestaurantService : IRestaurantService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public RestaurantService(AppDbContext context, IMapper mapper, IImageService imageService)
    {
        _context = context;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsAsync(string category = "", string address = null, string name = null)
    {
        var restaurants = await _context.Restaurant.OrderBy(r => r.RestaurantName).ToListAsync();

        if (!string.IsNullOrEmpty(name))
        {
            restaurants = restaurants.Where(r => r.RestaurantName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(address))
        {
            restaurants = restaurants.Where(r => r.Address.Contains(address, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(category))
        {
            restaurants = restaurants.Where(r => r.Type == category).ToList();
        }

        return restaurants;
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId)
    {
        return await _context.Restaurant.FirstOrDefaultAsync(r => r.RestaurantID == restaurantId);
    }

    public async Task<Restaurant> CreateRestaurantAsync(RestaurantDTO restaurantDTO)
    {
        var restaurant = _mapper.Map<Restaurant>(restaurantDTO);
        await _context.Restaurant.AddAsync(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }

    public async Task<bool> RestaurantExistsAsync(string restaurantName)
    {
        return await _context.Restaurant.AnyAsync(r => r.RestaurantName == restaurantName);
    }

    public async Task<IEnumerable<GetItems>> GetMenuAsync(int restaurantId, string sortByPrice = "")
    {
        var items = await _context.Item
            .Include(i => i.restaurant)
            .Where(i => i.RestaurantID == restaurantId)
            .ToListAsync();

        if (string.Equals(sortByPrice, "desc", StringComparison.OrdinalIgnoreCase))
        {
            items = items.OrderByDescending(i => i.ItemPrice).ToList();
        }
        else if (string.Equals(sortByPrice, "asc", StringComparison.OrdinalIgnoreCase))
        {
            items = items.OrderBy(i => i.ItemPrice).ToList();
        }

        return _mapper.Map<List<GetItems>>(items);
    }

    public async Task<Item> AddItemToMenuAsync(int restaurantId, ItemDTO itemDTO)
    {
        var newItem = _mapper.Map<Item>(itemDTO);
        newItem.RestaurantID = restaurantId;

        await _context.Item.AddAsync(newItem);
        await _context.SaveChangesAsync();
        return newItem;
    }

    public async Task<IEnumerable<GetItems>> GetAllItemsAsync(string itemName = "", string sortByPrice = "")
    {
        var items = await _context.Item
            .Include(i => i.restaurant)
            .OrderBy(r => r.ItemName)
            .ToListAsync();

        if (!string.IsNullOrEmpty(itemName))
        {
            items = items.Where(r => r.ItemName.Contains(itemName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(sortByPrice))
        {
            if (string.Equals(sortByPrice, "asc", StringComparison.OrdinalIgnoreCase))
            {
                items = items.OrderBy(i => i.ItemPrice).ToList();
            }
            else if (string.Equals(sortByPrice, "desc", StringComparison.OrdinalIgnoreCase))
            {
                items = items.OrderByDescending(i => i.ItemPrice).ToList();
            }
        }

        return _mapper.Map<List<GetItems>>(items);
    }

    public async Task UpdateImageUrlsAsync(int restaurantId, string[] urls)
    {
        var menu = await _context.Item
            .Where(i => i.RestaurantID == restaurantId)
            .ToListAsync();

        for (int i = 0; i < menu.Count && i < urls.Length; i++)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(menu[i].imageUrl))
            {
                await _imageService.DeleteImageAsync(menu[i].imageUrl);
            }
            
            menu[i].imageUrl = urls[i];
        }

        await _context.SaveChangesAsync();
    }
}