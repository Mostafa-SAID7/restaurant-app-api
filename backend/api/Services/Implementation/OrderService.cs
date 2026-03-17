using AutoMapper;
using FakeRestuarantAPI.Data;
using FakeRestuarantAPI.Models;
using FakeRestuarantAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FakeRestuarantAPI.Services.Implementation;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRestaurantService _restaurantService;

    public OrderService(AppDbContext context, IMapper mapper, IRestaurantService restaurantService)
    {
        _context = context;
        _mapper = mapper;
        _restaurantService = restaurantService;
    }

    public async Task<FullOrderDTO> CreateOrderAsync(int restaurantId, string apiKey, MenuDTO menuDTO)
    {
        var customer = await GetUserByApiKeyAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);
        if (restaurant == null)
            throw new ArgumentException($"No Restaurant Exists with {restaurantId}");

        var restaurantItems = await _context.Item
            .Where(i => i.RestaurantID == restaurantId)
            .ToListAsync();

        var orderList = new FullOrderDTO { fullorder = new List<Order>() };
        decimal grandTotal = 0.00m;

        var masterId = await GetNextMasterIdAsync();

        foreach (var item in menuDTO.menuDTO)
        {
            var itemExists = restaurantItems.FirstOrDefault(i => i.ItemName == item.ItemName);
            if (itemExists == null)
                throw new ArgumentException("The Item did not exist on restaurant menu");

            decimal totalPrice = itemExists.ItemPrice * item.Quantity;

            var order = _mapper.Map<Order>(item);
            order.UserID = customer.UserEmail;
            order.ItemPrice = itemExists.ItemPrice;
            order.TotalPrice = totalPrice;
            order.MasterID = masterId;

            grandTotal += totalPrice;
            orderList.fullorder.Add(order);
        }

        var masterOrder = new MasterOrder
        {
            UserID = customer.UserEmail,
            GrandTotal = grandTotal,
            RestaurantID = restaurant.RestaurantID
        };

        orderList.GrandTotal = grandTotal;

        await _context.masterOrders.AddAsync(masterOrder);
        await _context.SaveChangesAsync();

        foreach (var order in orderList.fullorder)
        {
            await _context.Order.AddAsync(order);
        }
        await _context.SaveChangesAsync();

        return orderList;
    }

    public async Task<IEnumerable<object>> GetUserOrdersAsync(string apiKey)
    {
        var customer = await GetUserByApiKeyAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        var totalOrders = await _context.masterOrders
            .Where(m => m.UserID == customer.UserEmail)
            .ToListAsync();

        return totalOrders.Select(t => new
        {
            masterID = t.MasterID,
            userID = t.UserID,
            usercode = t.user?.Usercode,
            RestaurantID = t.RestaurantID,
            Grandtotal = t.GrandTotal
        });
    }

    public async Task<IEnumerable<Order>> GetOrdersByMasterIdAsync(string apiKey, int masterId)
    {
        var customer = await GetUserByApiKeyAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        return await _context.Order
            .Where(o => o.MasterID == masterId)
            .ToListAsync();
    }

    public async Task<bool> DeleteOrderAsync(int orderId, string apiKey)
    {
        var customer = await GetUserByApiKeyAsync(apiKey);
        if (customer == null)
            return false;

        var order = await _context.Order.FirstOrDefaultAsync(o => o.OrderID == orderId);
        if (order == null)
            return false;

        _context.Order.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<object> DeleteMasterOrderAsync(int masterId, string apiKey)
    {
        var customer = await GetUserByApiKeyAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        var masterOrders = await _context.masterOrders
            .Include(o => o.restaurant)
            .Where(o => o.MasterID == masterId)
            .ToListAsync();

        if (!masterOrders.Any())
            throw new ArgumentException("Orders are not found with associated ID");

        var singleOrders = await _context.Order
            .Where(o => o.MasterID == masterId)
            .ToListAsync();

        _context.masterOrders.RemoveRange(masterOrders);
        _context.Order.RemoveRange(singleOrders);
        await _context.SaveChangesAsync();

        return new { message = "Master order Deleted", masterOrders, singleOrders };
    }

    public async Task<User?> GetUserByApiKeyAsync(string apiKey)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.Usercode == apiKey);
    }

    private async Task<int> GetNextMasterIdAsync()
    {
        var lastMasterId = await _context.masterOrders
            .OrderByDescending(m => m.MasterID)
            .Select(m => m.MasterID)
            .FirstOrDefaultAsync();

        return lastMasterId + 1;
    }
}