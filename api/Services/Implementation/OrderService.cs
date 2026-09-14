using AutoMapper;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories.Interfaces;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Services.Implementation;

/// <summary>
/// Order service for creating and managing orders
/// Phase A.1: Changed parameters from apiKey to userId (JWT)
/// </summary>
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates an order with proper transaction handling
    /// Phase A.1: userId from JWT claims instead of apiKey
    /// </summary>
    public async Task<OrderResponseDTO> CreateOrderAsync(int restaurantId, string userId, MenuDTO menuDTO)
    {
        // Validate input
        if (menuDTO?.menuDTO == null || menuDTO.menuDTO.Count == 0)
            throw new ArgumentException("Order must contain at least one item");

        var customer = await _unitOfWork.Users.GetByIdAsync(userId);
        if (customer == null)
            throw new UnauthorizedAccessException("User not found");

        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant not found with ID {restaurantId}");

        var restaurantItems = await _unitOfWork.Items.GetByRestaurantIdAsync(restaurantId);
        if (!restaurantItems.Any())
            throw new ArgumentException($"Restaurant menu is empty");

        // Validate all items exist and collect order lines
        var orderLines = new List<Order>();
        decimal grandTotal = 0.00m;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var item in menuDTO.menuDTO)
            {
                // Validate quantity
                if (item.Quantity < 1 || item.Quantity > 100)
                    throw new ArgumentException("Quantity must be between 1 and 100");

                // Find item by ItemID (preferred) or ItemName (fallback)
                Item itemExists = null;
                
                if (item.ItemID.HasValue && item.ItemID.Value > 0)
                {
                    itemExists = restaurantItems.FirstOrDefault(i => i.ItemID == item.ItemID.Value);
                    if (itemExists == null)
                        throw new ArgumentException($"Item with ID {item.ItemID} not found in restaurant menu");
                }
                else if (!string.IsNullOrWhiteSpace(item.ItemName))
                {
                    itemExists = restaurantItems.FirstOrDefault(i => i.ItemName == item.ItemName);
                    if (itemExists == null)
                        throw new ArgumentException($"Item '{item.ItemName}' not found in restaurant menu");
                }
                else
                {
                    throw new ArgumentException("ItemID or ItemName must be provided");
                }

                // Create order line with snapshot of item data at purchase time
                decimal totalPrice = itemExists.ItemPrice * item.Quantity;

                var order = new Order
                {
                    UserID = customer.Usercode,  // Use Usercode for consistency
                    ItemID = itemExists.ItemID,
                    ItemName = itemExists.ItemName,
                    ItemPrice = itemExists.ItemPrice,
                    Quantity = item.Quantity,
                    TotalPrice = totalPrice,
                    // MasterID will be set after MasterOrder is created and persisted
                };

                orderLines.Add(order);
                grandTotal += totalPrice;
            }

            // Create master order (header)
            var masterOrder = new MasterOrder
            {
                UserID = customer.Usercode,  // Use Usercode for consistency
                RestaurantID = restaurant.RestaurantID,
                GrandTotal = grandTotal
            };

            // Add master order first so it gets an ID assigned
            await _unitOfWork.MasterOrders.AddAsync(masterOrder);
            await _unitOfWork.SaveChangesAsync();  // This assigns masterOrder.MasterID

            // Now update all order lines with the generated MasterID
            foreach (var order in orderLines)
            {
                order.MasterID = masterOrder.MasterID;
            }

            // Add order lines
            await _unitOfWork.Orders.AddRangeAsync(orderLines);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Build response DTO (no entity exposure)
            var responseDTO = new OrderResponseDTO
            {
                MasterID = masterOrder.MasterID,
                GrandTotal = masterOrder.GrandTotal,
                CreatedAt = masterOrder.CreatedAt,
                Items = orderLines.Select(o => new OrderLineDTO
                {
                    OrderID = o.OrderID,
                    ItemID = o.ItemID,
                    ItemName = o.ItemName,
                    ItemPrice = o.ItemPrice,
                    Quantity = o.Quantity,
                    TotalPrice = o.TotalPrice,
                    MasterID = o.MasterID
                }).ToList()
            };

            return responseDTO;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<object>> GetUserOrdersAsync(string userId)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(userId);
        if (customer == null)
            throw new UnauthorizedAccessException("User not found");

        var totalOrders = await _unitOfWork.MasterOrders.GetWithRestaurantByUserIdAsync(customer.Usercode);

        return totalOrders.Select(t => new
        {
            masterID = t.MasterID,
            usercode = t.User?.Usercode,
            RestaurantID = t.RestaurantID,
            RestaurantName = t.Restaurant?.RestaurantName,
            GrandTotal = t.GrandTotal,
            CreatedAt = t.CreatedAt
        });
    }

    public async Task<IEnumerable<Order>> GetOrdersByMasterIdAsync(string userId, int masterId)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(userId);
        if (customer == null)
            throw new UnauthorizedAccessException("User not found");

        return await _unitOfWork.Orders.GetByMasterIdAsync(masterId);
    }

    public async Task<bool> DeleteOrderAsync(int orderId, string userId)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(userId);
        if (customer == null)
            return false;

        var deleted = await _unitOfWork.Orders.DeleteAsync(orderId);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return deleted;
    }

    public async Task<object> DeleteMasterOrderAsync(int masterId, string userId)
    {
        var customer = await _unitOfWork.Users.GetByIdAsync(userId);
        if (customer == null)
            throw new UnauthorizedAccessException("User not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var masterOrder = await _unitOfWork.MasterOrders.GetWithDetailsAsync(masterId);
            if (masterOrder == null)
                throw new ArgumentException("Master order not found");

            // Verify the master order belongs to this user
            if (masterOrder.UserID != customer.Usercode)
                throw new UnauthorizedAccessException("You do not have permission to delete this order");

            var singleOrders = await _unitOfWork.Orders.GetByMasterIdAsync(masterId);

            await _unitOfWork.MasterOrders.DeleteAsync(masterOrder);
            await _unitOfWork.Orders.DeleteRangeAsync(singleOrders);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new { message = "Master order deleted successfully", masterID = masterId };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
