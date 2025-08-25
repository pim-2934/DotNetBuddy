namespace DotNetBuddy.Domain;

/// <summary>
/// Defines the Unit of Work pattern to manage database transactions and repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// Represents a repository interface for managing the storage, retrieval, and manipulation of entities.
    /// <typeparam name="T">The type of the entity being managed.</typeparam>
    /// <typeparam name="TKey">The type of the primary key of the entity.</typeparam>
    /// <returns>A repository instance for performing operations on the specified entity type.</returns>
    IRepository<T, TKey> Repository<T, TKey>() where T : class, IEntity<TKey>;

    /// Asynchronously saves all changes made in the current unit of work's context to the underlying database.
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}