using Microsoft.EntityFrameworkCore.Storage;
using RestaurantAPI.Domain.Interfaces;
using RestaurantAPI.Infrastructure.Persistence.Repositories;

namespace RestaurantAPI.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation that coordinates repository operations and transactions.
/// Implements the IUnitOfWork contract from Domain layer.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IUserRepository? _userRepository;
    private IRestaurantRepository? _restaurantRepository;
    private IItemRepository? _itemRepository;
    private IOrderRepository? _orderRepository;
    private IMasterOrderRepository? _masterOrderRepository;
    private ICartRepository? _cartRepository;
    private IRoleRepository? _roleRepository;
    private IUserRoleRepository? _userRoleRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Repository properties (lazy initialization)
    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public IRestaurantRepository Restaurants => _restaurantRepository ??= new RestaurantRepository(_context);
    public IItemRepository Items => _itemRepository ??= new ItemRepository(_context);
    public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);
    public IMasterOrderRepository MasterOrders => _masterOrderRepository ??= new MasterOrderRepository(_context);
    public ICartRepository Carts => _cartRepository ??= new CartRepository(_context);
    public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);
    public IUserRoleRepository UserRoles => _userRoleRepository ??= new UserRoleRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

    // Transaction methods
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    // Bulk operations (not frequently used - can be implemented later if needed)
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        // TODO: Implement raw SQL if needed
        return await Task.FromResult(0);
    }

    public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, params object[] parameters) where T : class
    {
        // TODO: Implement raw SQL queries if needed
        return await Task.FromResult(new List<T>());
    }

    // Dispose pattern
    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
