using DotNetBuddy.Domain.Enums;

namespace DotNetBuddy.Domain;

/// <summary>
/// Provides a repository interface for managing CRUD operations and querying entities
/// using specification patterns within the domain layer.
/// </summary>
/// <typeparam name="T">The type of the entity managed by the repository.</typeparam>
/// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
public interface IRepository<T, TKey> where T : class, IEntity<TKey>
{
    /// <summary>
    /// Asynchronously retrieves a range of entities from the repository.
    /// </summary>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of entities retrieved from the repository.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a range of items from the specified queryable source.
    /// </summary>
    /// <param name="queryable">The queryable data source used to retrieve the range of items.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a collection of items retrieved from the queryable source.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a range of entities by their identifiers.
    /// </summary>
    /// <param name="ids">A collection of identifiers for the entities to retrieve.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the retrieved collection of entities.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a paged collection of items from the queryable data source.
    /// </summary>
    /// <param name="queryable">The queryable data source used to retrieve the paged items.</param>
    /// <param name="page">The page index (starting from 1) for retrieving the paged items.</param>
    /// <param name="pageSize">The number of items per page to retrieve.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a paged collection of items.</returns>
    Task<IEntityPagedResult<T, TKey>> GetPagedAsync(IQueryable<T> queryable, int page, int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a single entity from the specified queryable source.
    /// </summary>
    /// <param name="queryable">The queryable data source used to retrieve the entity.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the entity retrieved from the queryable source, or null if no entity is found.</returns>
    Task<T?> GetAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a single entity from the data source based on the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the entity matching the specified identifier, or null if not found.</returns>
    Task<T?> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements in the provided queryable source satisfy the specified criteria.
    /// </summary>
    /// <param name="queryable">The queryable data source to evaluate the criteria against.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if any elements meet the criteria; otherwise, false.</returns>
    Task<bool> AnyAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any entity with the specified identifier exists in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to check for existence.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean value indicating whether an entity with the specified identifier exists.</returns>
    Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a new entity to the data source.
    /// </summary>
    /// <param name="entity">The entity to be added to the data source.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the add operation.</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a collection of entities to the data source.
    /// </summary>
    /// <param name="entities">The collection of entities to add to the data source.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of the added entities.</returns>
    Task<IReadOnlyList<T>> AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the shallow (non-navigation) properties of an entity in the data source.
    /// </summary>
    /// <param name="entity">The entity with updated shallow properties to be saved to the data source.</param>
    void UpdateShallow(T entity);

    /// <summary>
    /// Updates an object and its related nested entities within the data source to reflect the provided state.
    /// </summary>
    /// <param name="entity">The primary entity to be updated along with its related nested data.</param>
    void UpdateDeep(T entity);

    /// <summary>
    /// Asynchronously deletes an entity from the data source based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="forceHardDelete">A boolean indicating whether to perform a hard delete of the entity. Defaults to false.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, indicating the success or failure of the delete operation.</returns>
    Task DeleteAsync(TKey id, bool forceHardDelete = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes a range of items from the data source based on the specified identifiers.
    /// </summary>
    /// <param name="ids">The collection of identifiers of the items to be deleted.</param>
    /// <param name="forceHardDelete">A boolean value indicating whether to bypass soft deletion and perform a hard delete.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous delete operation, indicating the completion of the operation.</returns>
    Task DeleteRangeAsync(IEnumerable<TKey> ids, bool forceHardDelete = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously counts the total number of entities in the repository.
    /// </summary>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the total count of entities.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously counts the number of entities in the specified queryable source.
    /// </summary>
    /// <param name="queryable">The queryable data source used to filter the entities for counting.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the total count of entities matching the specified criteria.</returns>
    Task<int> CountAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Provides an IQueryable interface to construct and execute LINQ queries on the repository, enabling advanced query composition.
    /// </summary>
    /// <returns>An IQueryable instance representing the collection of entities in the repository, allowing for further query building.</returns>
    IQueryable<T> MakeQuery(QueryOptions options = QueryOptions.None);
}