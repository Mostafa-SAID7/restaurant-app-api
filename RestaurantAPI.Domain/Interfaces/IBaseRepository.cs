using System.Linq.Expressions;
using RestaurantAPI.Domain.Entities;

namespace RestaurantAPI.Domain.Interfaces;

/// <summary>
/// Base repository interface with common CRUD operations.
/// This is the contract that all repositories must fulfill.
/// 
/// Repositories are data-access abstractions with NO business logic:
/// - Query/filter entities from database
/// - Insert/update/delete entities in database
/// - Include navigation properties for eager loading
/// 
/// Repositories do NOT:
/// - Validate business rules (validation belongs in Application layer)
/// - Transform entities to DTOs (mapping belongs in Application layer)
/// - Execute business logic (belongs in Application layer/handlers)
/// - Call other services (would break layering)
/// 
/// Repositories return ENTITIES only. Application layer handles entity→DTO conversion.
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
