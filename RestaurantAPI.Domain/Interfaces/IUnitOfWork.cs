namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions and repository access.
/// Provides a single gateway for all repository operations and transaction management.
/// Implements the Unit of Work pattern to maintain consistency across multiple repositories.
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
