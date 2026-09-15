using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Cart.Queries;

/// <summary>
/// Handler for GetCartSummaryQuery.
/// Retrieves complete cart with total calculation.
/// </summary>
public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCartSummaryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var cartItems = await _unitOfWork.Carts.GetByUserIdAsync(user.Usercode);

        var cartDto = new CartDto
        {
            CartItems = _mapper.Map<List<CartItemDto>>(cartItems),
            GrandTotal = cartItems.Sum(c => c.ItemPrice * c.Quantity)
        };

        return cartDto;
    }
}
