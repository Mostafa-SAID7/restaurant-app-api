namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions and repository access
/// Phase A.6: Added IRoleRepository, IUserRoleRepository, IRefreshTokenRepository for AuthService DIP
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    IUserRepository Users { get; }
    IRestaurantRepository Restaurants { get; }
    IItemRepository Items { get; }
    IOrderRepository Orders { get; }
    IMasterOrderRepository MasterOrders { get; }
    ICartRepository Carts { get; }
    IRoleRepository Roles { get; }
    IUserRoleRepository UserRoles { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    // Transaction methods
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    
    // Bulk operations
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, params object[] parameters) where T : class;
}