using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Orders.Commands;

/// <summary>
/// Handler for CreateOrderCommand.
/// Orchestrates order creation: validation → transaction → snapshots → persistence.
/// This is where ORDER CREATION BUSINESS LOGIC lives (moved from OrderService).
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CreateOrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (request.OrderData?.Items == null || request.OrderData.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item");

        var customer = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (customer == null)
            throw new UnauthorizedAccessException("User not found");

        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant not found with ID {request.RestaurantId}");

        var restaurantItems = await _unitOfWork.Items.GetByRestaurantIdAsync(request.RestaurantId);
        if (!restaurantItems.Any())
            throw new ArgumentException("Restaurant menu is empty");

        // Validate all items and collect order lines
        var orderLines = new List<Order>();
        decimal grandTotal = 0.00m;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var item in request.OrderData.Items)
            {
                // Validate quantity
                if (item.Quantity < 1 || item.Quantity > 100)
                    throw new ArgumentException("Quantity must be between 1 and 100");

                // Find item by ItemID (preferred) or ItemName (fallback)
                Item? itemExists = null;

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
                    UserID = customer.Usercode,
                    ItemID = itemExists.ItemID,
                    ItemName = itemExists.ItemName,
                    ItemPrice = itemExists.ItemPrice,
                    Quantity = item.Quantity,
                    TotalPrice = totalPrice,
                };

                orderLines.Add(order);
                grandTotal += totalPrice;
            }

            // Create master order (header)
            var masterOrder = new MasterOrder
            {
                UserID = customer.Usercode,
                RestaurantID = restaurant.RestaurantID,
                GrandTotal = grandTotal,
                Orders = orderLines  // Establish the relationship in-memory
            };

            // Add both master order and order lines in a single operation
            // MasterOrder is added first (parent), then all Orders (children)
            await _unitOfWork.MasterOrders.AddAsync(masterOrder);
            await _unitOfWork.Orders.AddRangeAsync(orderLines);

            // Single SaveChanges call ensures atomicity:
            // - Master order is inserted and ID assigned
            // - Order lines are inserted with the generated MasterID
            // - All-or-nothing persistence
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Build response using AutoMapper (no manual DTO construction)
            var responseDto = new CreateOrderResponseDto
            {
                MasterID = masterOrder.MasterID,
                GrandTotal = masterOrder.GrandTotal,
                CreatedAt = masterOrder.CreatedAt,
                Items = _mapper.Map<List<OrderLineDto>>(orderLines)
            };

            return responseDto;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
