using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Restaurants.Queries;

/// <summary>
/// Handler for GetAllItemsQuery.
/// Retrieves all menu items across all restaurants with optional filtering and sorting.
/// </summary>
public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, IEnumerable<ItemResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ItemResponseDto>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        // Get items with optional filters
        var items = await _unitOfWork.Items.GetByFiltersAsync(
            request.ItemName ?? "",
            request.SortByPrice ?? ""
        );

        return _mapper.Map<IEnumerable<ItemResponseDto>>(items);
    }
}
