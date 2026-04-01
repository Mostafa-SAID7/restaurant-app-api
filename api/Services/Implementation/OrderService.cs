using AutoMapper;
using RestuarantAPI.Models;
using RestuarantAPI.Repositories.Interfaces;
using RestuarantAPI.Services.Interfaces;

namespace RestuarantAPI.Services.Implementation;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FullOrderDTO> CreateOrderAsync(int restaurantId, string apiKey, MenuDTO menuDTO)
    {
        var customer = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
        if (restaurant == null)
            throw new ArgumentException($"No Restaurant Exists with {restaurantId}");

        var restaurantItems = await _unitOfWork.Items.GetByRestaurantIdAsync(restaurantId);

        var orderList = new FullOrderDTO { fullorder = new List<Order>() };
        decimal grandTotal = 0.00m;

        var masterId = await _unitOfWork.Orders.GetNextMasterIdAsync();

        await _unitOfWork.BeginTransactionAsync();
        try
        {
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

            await _unitOfWork.MasterOrders.AddAsync(masterOrder);
            await _unitOfWork.Orders.AddRangeAsync(orderList.fullorder);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return orderList;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<object>> GetUserOrdersAsync(string apiKey)
    {
        var customer = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        var totalOrders = await _unitOfWork.MasterOrders.GetWithRestaurantByUserIdAsync(customer.UserEmail);

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
        var customer = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        return await _unitOfWork.Orders.GetByMasterIdAsync(masterId);
    }

    public async Task<bool> DeleteOrderAsync(int orderId, string apiKey)
    {
        var customer = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (customer == null)
            return false;

        var deleted = await _unitOfWork.Orders.DeleteAsync(orderId);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        return deleted;
    }

    public async Task<object> DeleteMasterOrderAsync(int masterId, string apiKey)
    {
        var customer = await _unitOfWork.Users.GetByUserCodeAsync(apiKey);
        if (customer == null)
            throw new UnauthorizedAccessException("Invalid API key");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var masterOrder = await _unitOfWork.MasterOrders.GetWithDetailsAsync(masterId);
            if (masterOrder == null)
                throw new ArgumentException("Orders are not found with associated ID");

            var singleOrders = await _unitOfWork.Orders.GetByMasterIdAsync(masterId);

            await _unitOfWork.MasterOrders.DeleteAsync(masterOrder);
            await _unitOfWork.Orders.DeleteRangeAsync(singleOrders);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return new { message = "Master order Deleted", masterOrder, singleOrders };
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}