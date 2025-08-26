using DotNetBuddy.Domain.Enums;

namespace DotNetBuddy.Domain;

/// <summary>
/// Defines a repository interface for performing data access operations, including CRUD operations,
/// querying, and pagination, while supporting filtering and query configuration.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being managed.</typeparam>
/// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Asynchronously retrieves a range of entities from the repository with the specified query options.
    /// </summary>
    /// <param name="queryOptions">Optional. Defines query options such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of entities retrieved from the repository.</returns>
    Task<IReadOnlyList<TEntity>> GetRangeAsync(
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a range of entities from the repository using the specified query configuration.
    /// </summary>
    /// <param name="configureQuery">A function to configure the queryable source used to retrieve the entities.</param>
    /// <param name="queryOptions">Optional. Specifies additional query options, such as including soft-deleted records.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of entities retrieved from the repository.</returns>
    Task<IReadOnlyList<TEntity>> GetRangeAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a range of entities by their identifiers.
    /// </summary>
    /// <param name="ids">A collection of identifiers for the entities to retrieve.</param>
    /// <param name="queryOptions">Optional. Defines additional query options, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of the retrieved entities.</returns>
    Task<IReadOnlyList<TEntity>> GetRangeAsync(
        IEnumerable<TKey> ids,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a range of entities based on the specified identifiers, queryable source, and query options.
    /// </summary>
    /// <param name="ids">The collection of entity identifiers to retrieve.</param>
    /// <param name="configureQuery">A function to configure the queryable source for filtering or projecting the entities.</param>
    /// <param name="queryOptions">Optional. An enumeration specifying additional query options, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of entities matching the specified criteria.</returns>
    Task<IReadOnlyList<TEntity>> GetRangeAsync(
        IEnumerable<TKey> ids,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a paged result of entities from the repository.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <param name="queryOptions">Optional. Specifies additional query options, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the paged result of entities.</returns>
    Task<IEntityPagedResult<TEntity, TKey>> GetPagedAsync(
        int page,
        int pageSize,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a paged collection of entities from the repository
    /// based on the specified query configuration and pagination parameters.
    /// </summary>
    /// <param name="configureQuery">A function to customize the queryable used to retrieve the data.</param>
    /// <param name="page">The page index (starting at 1) of the collection to retrieve.</param>
    /// <param name="pageSize">The number of records to include in each page.</param>
    /// <param name="queryOptions">Optional. Specified options to modify the query, such as including soft-deleted items.</param>
    /// <param name="cancellationToken">Optional. A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing a paged result of entities based on the query configuration.</returns>
    Task<IEntityPagedResult<TEntity, TKey>> GetPagedAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        int page,
        int pageSize,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a single entity based on the specified query configuration from the repository.
    /// </summary>
    /// <param name="configureQuery">A function to configure the query to filter the entity.</param>
    /// <param name="queryOptions">Optional. Specifies query behavior, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the retrieved entity or null if no entity is found.</returns>
    Task<TEntity?> GetAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves a single entity from the repository based on the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="queryOptions">Optional. Specifies additional query options such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the entity that matches the specified identifier, or null if no such entity is found.</returns>
    Task<TEntity?> GetAsync(
        TKey id,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously retrieves an entity that matches the specified identifier and applies the provided query configuration.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="configureQuery">A function to configure additional filtering or operations on the queryable source.</param>
    /// <param name="queryOptions">The query options to control additional behaviors, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity matching the specified identifier and query configuration, or null if no such entity is found.</returns>
    Task<TEntity?> GetAsync(
        TKey id,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously determines whether any elements in the provided queryable source satisfy the specified criteria.
    /// </summary>
    /// <param name="configureQuery">A function to configure the queryable data source.</param>
    /// <param name="queryOptions">Optional. The query options to apply, such as including soft-deleted items.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if any elements meet the criteria; otherwise, false.</returns>
    Task<bool> AnyAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously determines whether any entity with the specified identifier exists in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to check for existence.</param>
    /// <param name="queryOptions">Optional. Specifies additional query behaviors, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean value indicating whether an entity with the specified identifier exists.</returns>
    Task<bool> AnyAsync(
        TKey id,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to be added to the repository.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the added entity.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a collection of entities to the data source.
    /// </summary>
    /// <param name="entities">The collection of entities to add to the data source.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of the added entities.</returns>
    Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the shallow (non-navigation) properties of an entity in the data source.
    /// </summary>
    /// <param name="entity">The entity with updated shallow properties to be saved to the data source.</param>
    void UpdateShallow(TEntity entity);

    /// <summary>
    /// Updates the specified entity and all its related nested entities in the repository, ensuring that the entire object graph is persistently updated.
    /// </summary>
    /// <param name="entity">The primary entity along with its associated nested entities to update in the data store.</param>
    void UpdateDeep(TEntity entity);

    /// <summary>
    /// Asynchronously deletes an entity from the data source based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="forceHardDelete">A boolean indicating whether to perform a hard delete of the entity. Defaults to false.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, indicating the success or failure of the delete operation.</returns>
    Task DeleteAsync(TKey id, bool forceHardDelete = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes a range of entities from the data source based on the provided identifiers.
    /// </summary>
    /// <param name="ids">The collection of identifiers of the entities to be deleted.</param>
    /// <param name="forceHardDelete">A flag indicating whether to perform a hard delete, bypassing any soft delete functionality.</param>
    /// <param name="cancellationToken">An optional cancellation token to observe while performing the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteRangeAsync(IEnumerable<TKey> ids, bool forceHardDelete = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously counts the total number of entities in the repository.
    /// </summary>
    /// <param name="queryOptions">Optional. Specifies query behavior, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the total count of entities.</returns>
    Task<int> CountAsync(
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously counts the number of entities based on the specified query configuration and options.
    /// </summary>
    /// <param name="configureQuery">A function to configure the queryable source, such as applying filters or projections.</param>
    /// <param name="queryOptions">Optional. Flags indicating any additional query processing options, such as including soft-deleted entities.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the total count of entities matching the specified criteria.</returns>
    Task<int> CountAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default
    );
}