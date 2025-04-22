namespace BuddyDotNet;

/// <summary>
/// Defines the Unit of Work pattern to manage database transactions and repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// Interface representing a unit of work that encapsulates multiple repository operations within a single transaction.
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <returns>A repository instance for the specified entity type.</returns>
    IRepository<T> Repository<T>() where T : class, IEntity;

    /// Asynchronously saves all changes made in the current unit of work's context to the underlying database.
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveAsync();
}