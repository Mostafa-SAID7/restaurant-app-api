using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Application.Features.Orders.Authorization;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Orders.Queries;

/// <summary>
/// Handler for GetOrderByMasterIdQuery.
/// Retrieves order lines for a master order, mapped using AutoMapper.
/// 
/// Authorization: Uses IOrderAuthorizationService to verify user owns the master order
/// before retrieving its line items. This ensures users can only access their own orders.
/// </summary>
public class GetOrderByMasterIdQueryHandler : IRequestHandler<GetOrderByMasterIdQuery, List<OrderLineDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IOrderAuthorizationService _orderAuthorizationService;

    public GetOrderByMasterIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IOrderAuthorizationService orderAuthorizationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _orderAuthorizationService = orderAuthorizationService;
    }

    public async Task<List<OrderLineDto>> Handle(GetOrderByMasterIdQuery request, CancellationToken cancellationToken)
    {
        // Verify user is authorized and owns the master order
        await _orderAuthorizationService.AuthorizeMasterOrderOwnershipAsync(request.UserId, request.MasterId);

        var orders = await _unitOfWork.Orders.GetByMasterIdAsync(request.MasterId);
        return _mapper.Map<List<OrderLineDto>>(orders);
    }
}
