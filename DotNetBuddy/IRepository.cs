using System.Linq.Expressions;
using BuddyDotNet.Enums;

namespace BuddyDotNet;

/// <summary>
/// Defines a generic repository interface for managing data operations on entities.
/// </summary>
/// <typeparam name="T">The type of entity that the repository manages, which must implement the <see cref="IEntity"/> interface.</typeparam>
public interface IRepository<T> where T : class, IEntity
{
    /// <summary>
    /// Retrieves a list of entities from the repository, optionally filtered by a predicate and including navigation properties.
    /// </summary>
    /// <param name="predicate">An optional condition to filter the entities. Defaults to null if no filtering is required.</param>
    /// <param name="options">Optional query options that modify the behavior of the query execution.</param>
    /// <param name="includes">An array of expressions specifying related entities to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of entities that match the specified criteria.</returns>
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Retrieves a single entity from the repository that matches the specified predicate, optionally including navigation properties.
    /// </summary>
    /// <param name="predicate">The condition used to filter the entity to retrieve.</param>
    /// <param name="options">Query options that modify the behavior of the query execution.</param>
    /// <param name="includes">An array of expressions specifying related entities to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity that matches the specified condition, or null if no such entity is found.</returns>
    Task<T?> GetAsync(
        Expression<Func<T?, bool>> predicate,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Retrieves an entity by its unique identifier from the repository, optionally including specified navigation properties.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="options">Optional query options that modify the behavior of the query execution.</param>
    /// <param name="includes">An array of expressions specifying related entities to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found, or null if no entity matches the specified identifier.</returns>
    Task<T?> GetAsync(
        Guid id,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Determines whether any entities in the repository match the specified predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="options">Optional query options that modify the behavior of the query execution. Defaults to QueryOptions.None.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a value indicating whether any entities satisfy the condition specified by the predicate.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryOptions options = QueryOptions.None);

    /// <summary>
    /// Determines whether any entities exist in the repository that match the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to search for.</param>
    /// <param name="options">Optional query options that modify the behavior of the query execution.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether an entity with the specified ID exists.</returns>
    Task<bool> AnyAsync(Guid id, QueryOptions options = QueryOptions.None);
    
    /// <summary>
    /// Adds a new entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be added to the repository.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Adds a new entity to the repository and persists it to the database.
    /// </summary>
    /// <param name="entities">The entities to be added to the repository.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
    Task<List<T>> AddAsync(List<T> entities);

    /// <summary>
    /// Updates the given entity in the repository and optionally overwrites its relationships.
    /// </summary>
    /// <param name="entity">The entity to update in the repository.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity from the repository based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}