using MediatR;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Application.Features.Restaurants.Commands;

/// <summary>
/// Handler for DeleteRestaurantCommand.
/// Deletes a restaurant from the system.
/// </summary>
public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRestaurantCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (request.RestaurantId <= 0)
            throw new ArgumentException("Restaurant ID must be greater than 0");

        // Get existing restaurant
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId);
        if (restaurant == null)
            throw new ArgumentException($"Restaurant with ID {request.RestaurantId} not found");

        // Delete
        var deleted = await _unitOfWork.Restaurants.DeleteAsync(restaurant);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return deleted;
    }
}
