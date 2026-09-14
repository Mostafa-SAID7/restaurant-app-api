using System.Linq.Expressions;

namespace RestaurantAPI.Repositories.Interfaces;

/// <summary>
/// Base repository interface with common CRUD operations.
/// Phase A.7: Pure data access layer - no business logic allowed.
/// 
/// CONSTRAINTS:
/// - Repositories MUST NOT contain validation logic
/// - Repositories MUST NOT contain business calculations or transformations
/// - Repositories MUST NOT call services (would create circular dependencies)
/// - Repositories MUST NOT throw business exceptions (use data exceptions only)
/// - Repositories SHOULD only query, insert, update, or delete data
/// - Entity mapping to DTOs happens in SERVICES, not repositories
/// - Complex data operations (sorting, filtering) are OK here - they're data-layer concerns
/// 
/// All business logic belongs in Services (IUserService, IAuthService, etc.)
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IBaseRepository<T> where T : class
{
    // Read operations
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    // Write operations
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    Task<T> UpdateAsync(T entity);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteAsync(T entity);
    Task<int> DeleteRangeAsync(IEnumerable<T> entities);
    Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate);

    // Query operations
    IQueryable<T> Query();
    IQueryable<T> Query(Expression<Func<T, bool>> predicate);
}