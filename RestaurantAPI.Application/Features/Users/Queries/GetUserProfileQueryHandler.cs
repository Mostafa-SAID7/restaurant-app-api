using AutoMapper;
using MediatR;
using RestaurantAPI.Application.Common.DTOs;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Users.Queries;

/// <summary>
/// Handler for GetUserProfileQuery.
/// Retrieves the current authenticated user's profile information.
/// </summary>
public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDTO?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDTO?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            throw new ArgumentException("User ID is required");

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            return null;

        return _mapper.Map<UserDTO>(user);
    }
}
