using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Commands;

/// <summary>
/// Handler for AddItemToCartCommand.
/// Handles business logic: user validation → item validation → cart persistence.
/// </summary>
public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, CartItemDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddItemToCartCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartItemDto> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // Fetch the Item by ID and validate it exists
        var item = await _unitOfWork.Items.GetByIdAsync(request.CartItem.ItemID);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {request.CartItem.ItemID} not found");

        var cart = new Domain.Entities.Cart
        {
            UserID = user.Usercode,
            ItemID = item.ItemID,
            ItemName = item.ItemName,
            ItemPrice = item.ItemPrice,
            Quantity = request.CartItem.Quantity
        };

        await _unitOfWork.Carts.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CartItemDto>(cart);
    }
}
