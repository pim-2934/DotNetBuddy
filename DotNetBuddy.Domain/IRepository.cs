using System.Linq.Expressions;
using DotNetBuddy.Domain.Enums;

namespace DotNetBuddy.Domain;

/// <summary>
/// Provides a generic repository interface for data access and management operations on entities.
/// </summary>
/// <typeparam name="T">The type of the entity, which must implement <see cref="IEntity{TKey}"/>.</typeparam>
/// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
public interface IRepository<T, TKey> where T : class, IEntity<TKey>
{
    /// <summary>
    /// Retrieves a collection of entities from the repository, optionally filtered by a predicate and including specified navigation properties.
    /// </summary>
    /// <param name="options">Query execution options that can modify the behavior of the operation. Defaults to QueryOptions.None.</param>
    /// <param name="includes">A set of expressions identifying navigation properties to include in the query results.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is an enumerable collection of entities matching the specified criteria.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Retrieves a collection of entities from the repository, optionally filtered by a predicate and including specified navigation properties.
    /// </summary>
    /// <param name="predicate">An expression to filter the entities. Defaults to null if no filtering is applied.</param>
    /// <param name="options">Query execution options that can modify the behavior of the operation. Defaults to QueryOptions.None.</param>
    /// <param name="includes">A set of expressions identifying navigation properties to include in the query results.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is an enumerable collection of entities matching the specified criteria.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(
        Expression<Func<T, bool>> predicate,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Retrieves a list of entities from the repository that match the specified identifiers, optionally including navigation properties and modifying query behavior.
    /// </summary>
    /// <param name="ids">A collection of entity identifiers to retrieve.</param>
    /// <param name="options">Optional query options that modify the behavior of the query execution.</param>
    /// <param name="includes">An array of expressions specifying related entities to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of entities that correspond to the specified identifiers.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(
        IEnumerable<TKey> ids,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Retrieves a paginated collection of entities from the repository based on the provided page number, page size, optional filter, and navigation properties.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve, where the first page is 1.</param>
    /// <param name="pageSize">The number of entities to include in each page.</param>
    /// <param name="predicate">An optional expression to filter the entities. Defaults to null if no filtering is applied.</param>
    /// <param name="options">Query execution options that can modify the behavior of the operation. Defaults to QueryOptions.None.</param>
    /// <param name="includes">A set of expressions identifying navigation properties to include in the query results.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is a paged result containing the entities and associated metadata matching the specified criteria.</returns>
    Task<IEntityPagedResult<T, TKey>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Retrieves a paged collection of entities from the repository based on the specified page number, size, and optional filtering criteria.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve. Must be greater than or equal to 1.</param>
    /// <param name="pageSize">The number of entities to include on each page. Must be greater than or equal to 1.</param>
    /// <param name="ids">A collection of entity identifiers to retrieve.</param>
    /// <param name="options">Query execution options that can modify the behavior of the operation. Defaults to QueryOptions.None.</param>
    /// <param name="includes">A set of expressions identifying navigation properties to include in the query results.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task contains a paged result including the specified entities and metadata related to pagination.</returns>
    Task<IEntityPagedResult<T, TKey>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        IEnumerable<TKey> ids,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Searches for entities that have searchable properties containing the specified search term.
    /// </summary>
    /// <param name="searchTerm">The term to search for in searchable properties.</param>
    /// <param name="options">Query execution options that can modify the behavior of the operation.</param>
    /// <param name="includes">A set of expressions identifying navigation properties to include in the query results.</param>
    /// <returns>A task representing the asynchronous operation. The result is a collection of entities matching the search criteria.</returns>
    Task<IReadOnlyList<T>> SearchAsync(
        string searchTerm,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Searches for entities that have searchable properties containing the specified search term, with pagination.
    /// </summary>
    /// <param name="searchTerm">The term to search for in searchable properties.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="options">Query execution options that can modify the behavior of the operation.</param>
    /// <param name="includes">A set of expressions identifying navigation properties to include in the query results.</param>
    /// <returns>A task representing the asynchronous operation. The result is a paginated collection of entities matching the search criteria.</returns>
    Task<IEntityPagedResult<T, TKey>> SearchPagedAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Retrieves a single entity from the repository that matches the specified predicate, optionally including related navigation properties.
    /// </summary>
    /// <param name="predicate">The condition used to filter the entity to retrieve.</param>
    /// <param name="options">Query options that can modify the query execution behavior. Defaults to QueryOptions.None.</param>
    /// <param name="includes">An array of expressions specifying navigation properties to include in the query results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity that matches the specified predicate, or null if no such entity is found.</returns>
    Task<T?> GetAsync(
        Expression<Func<T, bool>> predicate,
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
        TKey id,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Determines whether any entities in the repository satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a specific condition.</param>
    /// <param name="options">Query execution options that can modify the behavior of the operation. Defaults to QueryOptions.None.</param>
    /// <returns>A task that represents the asynchronous operation. The result of the task is a boolean value indicating whether any entities match the given predicate.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryOptions options = QueryOptions.None);

    /// <summary>
    /// Determines whether any entity with the specified ID exists in the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to search for.</param>
    /// <param name="options">Optional query options that modify the query execution behavior. Defaults to QueryOptions.None.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a boolean indicating whether an entity with the specified ID exists.</returns>
    Task<bool> AnyAsync(TKey id, QueryOptions options = QueryOptions.None);

    /// <summary>
    /// Adds a new entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be added to the repository.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Adds a single entity to the repository and persists it to the database.
    /// </summary>
    /// <param name="entities">The entities to be added to the repository.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
    Task<IReadOnlyList<T>> AddAsync(IEnumerable<T> entities);

    /// <summary>
    /// Updates the specified entity in the repository without modifying its related entities.
    /// </summary>
    /// <param name="entity">The entity to be updated in the repository.</param>
    void UpdateShallow(T entity);

    /// <summary>
    /// Updates an entity and its related entities in the repository.
    /// </summary>
    /// <param name="entity">The entity to update, including any associated related entities that need to be updated as part of the operation.</param>
    void UpdateDeep(T entity);

    /// <summary>
    /// Deletes an entity from the repository based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to be deleted.</param>
    /// <returns>A task that represents the asynchronous deletion operation.</returns>
    Task DeleteAsync(TKey id);

    /// <summary>
    /// Deletes a range of entities from the repository identified by their unique keys.
    /// </summary>
    /// <param name="ids">A collection of unique identifiers corresponding to the entities to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation of deleting the specified entities.</returns>
    Task DeleteRangeAsync(IEnumerable<TKey> ids);

    /// <summary>
    /// Counts the total number of entities in the repository that match the specified predicate.
    /// </summary>
    /// <param name="predicate">An optional expression to filter the entities to count. Defaults to null for counting all entities.</param>
    /// <param name="options">Optional query options that modify the behavior of the query execution.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities matching the predicate.</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, QueryOptions options = QueryOptions.None);
}