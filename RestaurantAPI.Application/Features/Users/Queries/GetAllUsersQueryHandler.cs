using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Users.Queries;

/// <summary>
/// Handler for GetAllUsersQuery.
/// Retrieves all registered users (admin-only operation).
/// 
/// SECURITY: This handler should be behind authorization policy restricting access to admins.
/// Use [Authorize(Policy = "AdminOnly")] on the controller endpoint.
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}
