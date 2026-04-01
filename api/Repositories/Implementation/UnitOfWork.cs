using RestuarantAPI.Data;
using RestuarantAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RestuarantAPI.Repositories.Implementation;

/// <summary>
/// Unit of Work implementation for managing transactions and repository access
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IUserRepository? _users;
    private IRestaurantRepository? _restaurants;
    private IItemRepository? _items;
    private IOrderRepository? _orders;
    private IMasterOrderRepository? _masterOrders;
    private ICartRepository? _carts;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRestaurantRepository Restaurants => _restaurants ??= new RestaurantRepository(_context);
    public IItemRepository Items => _items ??= new ItemRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IMasterOrderRepository MasterOrders => _masterOrders ??= new MasterOrderRepository(_context);
    public ICartRepository Carts => _carts ??= new CartRepository(_context);

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
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // Bulk operations
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, params object[] parameters) where T : class
    {
        return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
    }

    // Dispose pattern
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}