namespace RestaurantAPI.Data.Seeds;

/// <summary>
/// Interface for database seeding operations.
/// Implement for each domain to provide seed data.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Seed data into the database.
    /// Called during application startup or initialization.
    /// </summary>
    /// <param name="context">Database context</param>
    /// <returns>Task completion</returns>
    Task SeedAsync(AppDbContext context);
}
