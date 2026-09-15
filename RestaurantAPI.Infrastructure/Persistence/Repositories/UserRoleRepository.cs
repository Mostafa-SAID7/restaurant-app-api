using RestaurantAPI.Domain.Entities;
using RestaurantAPI.Domain.Interfaces;

namespace RestaurantAPI.Infrastructure.Persistence.Repositories;

public class UserRoleRepository : BaseRepository<ApplicationUserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }
}
