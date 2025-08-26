using System.Linq.Expressions;
using DotNetBuddy.Domain.Common;
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
    /// Asynchronously retrieves a range of items from the specified data source using the given query specification.
    /// </summary>
    /// <param name="spec">The query specification that defines the criteria for retrieving the range of items.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a collection of items that match the specified query criteria.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a range of elements from the data source based on the provided query options and included navigation properties.
    /// </summary>
    /// <param name="options">The query options that define behaviors such as tracking or splitting queries.</param>
    /// <param name="includes">An array of expressions specifying the related entities to include in the query results.</param>
    /// <returns>A task representing the asynchronous operation, containing a read-only list of elements that meet the specified conditions.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<IReadOnlyList<T>> GetRangeAsync(
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously retrieves a range of entities that match the specified predicate and query options.
    /// </summary>
    /// <param name="predicate">The function used to filter the entities to retrieve.</param>
    /// <param name="options">The query behavior options to apply, such as tracking or split queries.</param>
    /// <param name="includes">Expressions specifying the related entities to include in the results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of entities satisfying the predicate and query options.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<IReadOnlyList<T>> GetRangeAsync(
        Expression<Func<T, bool>> predicate,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously retrieves a range of entities by their identifiers based on the specified query specification.
    /// </summary>
    /// <param name="ids">A collection of entity identifiers to retrieve.</param>
    /// <param name="spec">The query specification that defines filtering, sorting, and other criteria.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the retrieved collection of entities.</returns>
    Task<IReadOnlyList<T>> GetRangeAsync(IEnumerable<TKey> ids, QuerySpecification<T> spec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a range of items based on specified identifiers, query options, and related entities to include in the results.
    /// </summary>
    /// <param name="ids">A collection of unique identifiers specifying the items to retrieve.</param>
    /// <param name="options">Query options that define how the query should be executed. Defaults to no special options.</param>
    /// <param name="includes">An optional array of expressions specifying related entities to include in the result.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of the retrieved items.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<IReadOnlyList<T>> GetRangeAsync(
        IEnumerable<TKey> ids,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously retrieves a paged collection of items from the data source based on the given query specification.
    /// </summary>
    /// <param name="spec">The query specification that defines the criteria and configuration for retrieving the items.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a paged collection of items meeting the specified criteria.</returns>
    Task<IEntityPagedResult<T, TKey>> GetPagedAsync(QuerySpecification<T> spec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a paginated collection of entities from the repository
    /// based on the specified page number, page size, optional filtering, and navigation properties.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve, where the first page is numbered 1.</param>
    /// <param name="pageSize">The size of each page indicating the number of entities to be included.</param>
    /// <param name="predicate">An optional filtering expression to apply to entities. If null, no filtering is applied.</param>
    /// <param name="options">Query execution options used to customize the query behavior. Defaults to QueryOptions.None.</param>
    /// <param name="includes">Optional navigation property expressions indicating related data to include in the results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated result of entities along with associated metadata matching the supplied criteria.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<IEntityPagedResult<T, TKey>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously retrieves a paginated collection of items from a data source based on the specified parameters.
    /// </summary>
    /// <param name="pageNumber">The one-based number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items to include in a single page.</param>
    /// <param name="ids">Collection of identifiers to filter the results by specific items.</param>
    /// <param name="options">Specifies additional query options such as including deleted items or query behaviors.</param>
    /// <param name="includes">Expressions specifying related entities to include in the result.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated collection of items matching the specified criteria.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<IEntityPagedResult<T, TKey>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        IEnumerable<TKey> ids,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously searches for items in the data source that match the specified search term and query specification.
    /// </summary>
    /// <param name="searchTerm">The search term used to find matching items in the data source.</param>
    /// <param name="spec">The query specification that defines additional criteria for filtering or ordering the results.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a read-only list of items that match the search term and query specification.</returns>
    Task<IReadOnlyList<T>> SearchAsync(string searchTerm, QuerySpecification<T> spec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously searches for entities matching the specified search term and query options.
    /// </summary>
    /// <param name="searchTerm">The term to search for within the entities.</param>
    /// <param name="options">Additional options that modify the behavior of the query, such as tracking and caching settings.</param>
    /// <param name="includes">An array of expressions specifying related entities to include in the results.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a read-only list of entities that match the search criteria.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<IReadOnlyList<T>> SearchAsync(
        string searchTerm,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously performs a paged search operation based on the provided search term and query specification.
    /// </summary>
    /// <param name="searchTerm">The term to search for, used to filter results based on relevancy to the given term.</param>
    /// <param name="spec">The query specification defining additional filtering, sorting, and projection criteria for the search.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a paged collection of results that match the search term and specification.</returns>
    Task<IEntityPagedResult<T, TKey>> SearchPagedAsync(string searchTerm, QuerySpecification<T> spec,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a paginated collection of entities that match the specified search criteria.
    /// </summary>
    /// <param name="searchTerm">The term to use for filtering entities in the search.</param>
    /// <param name="pageNumber">The one-based index of the page to retrieve.</param>
    /// <param name="pageSize">The number of items to include in each page of results.</param>
    /// <param name="options">Additional options for customizing the query, such as ignoring case sensitivity or including soft-deleted records.</param>
    /// <param name="includes">Optional expressions specifying related entities to include in the result set.</param>
    /// <returns>A task that represents the asynchronous operation, containing a paginated result of matching entities.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<IEntityPagedResult<T, TKey>> SearchPagedAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously retrieves a single entity from the repository that matches the criteria defined in the provided specification.
    /// </summary>
    /// <param name="spec">An instance of QuerySpecification that specifies the filtering criteria for selecting the entity.</param>
    /// <param name="cancellationToken">An optional token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is the entity that matches the specified criteria, or null if no match is found.</returns>
    Task<T?> GetAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a single item from the data source based on a specified predicate and optional query options or includes.
    /// </summary>
    /// <param name="predicate">The condition to be satisfied by the item to retrieve.</param>
    /// <param name="options">Optional query behavior modifiers, such as whether to track changes or ignore query filters.</param>
    /// <param name="includes">Optional navigation properties to include in the query result.</param>
    /// <returns>A task that represents the asynchronous operation, containing the single requested item if found, or null if no item matches the criteria.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<T?> GetAsync(
        Expression<Func<T, bool>> predicate,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously retrieves a single item from a data source based on the specified identifier and query criteria.
    /// </summary>
    /// <param name="id">The identifier of the item to retrieve.</param>
    /// <param name="spec">The query specification containing optional criteria for filtering or including related data.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation, containing the item matching the specified identifier and criteria, or null if not found.</returns>
    Task<T?> GetAsync(TKey id, QuerySpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a single entity from the data source using the specified identifier and query options.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="options">Query execution options to modify the behavior, such as tracking or filtering.</param>
    /// <param name="includes">Expressions specifying related entities to include in the query results.</param>
    /// <returns>A task that represents the asynchronous operation, containing the entity that matches the given identifier, or null if not found.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<T?> GetAsync(
        TKey id,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    );

    /// <summary>
    /// Asynchronously determines if any entities in the repository satisfy the criteria specified in the given query specification.
    /// </summary>
    /// <param name="spec">An instance of QuerySpecification specifying the criteria for evaluating entities in the repository.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The result of the task is a boolean indicating whether any entities match the specified criteria.</returns>
    Task<bool> AnyAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements in the repository satisfy the condition specified by the given predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="options">Options for modifying the behavior of the query, such as disabling tracking or using split queries.</param>
    /// <returns>A task that represents the asynchronous operation, resulting in a boolean value that indicates whether any elements match the criterion defined by the predicate.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryOptions options = QueryOptions.None);

    /// <summary>
    /// Asynchronously determines whether any entity in the repository satisfies the specified condition.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="spec">The query specification that defines the conditions to evaluate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests while the operation is in progress.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean value indicating whether any entity meets the specified condition.</returns>
    Task<bool> AnyAsync(TKey id, QuerySpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines whether any elements identified by the specific key exist in the data set, optionally applying query options.
    /// </summary>
    /// <param name="id">The unique identifier of the element to check for existence.</param>
    /// <param name="options">Optional query behavior modifiers, such as tracking or filtering options.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether an element with the specified key exists in the data set.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<bool> AnyAsync(TKey id, QueryOptions options = QueryOptions.None);

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
    /// Asynchronously counts the number of entities in the repository that satisfy the conditions specified in the given query specification.
    /// </summary>
    /// <param name="spec">The query specification that defines the criteria for filtering the entities to be counted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the total count of entities matching the criteria.</returns>
    Task<int> CountAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously counts the total number of entities in the repository that match the specified predicate.
    /// </summary>
    /// <param name="predicate">An optional expression to filter the entities. If null, counts all entities in the repository.</param>
    /// <param name="options">Specifies query execution options, such as tracking behavior or query filters, that modify how the operation is executed. Defaults to QueryOptions.None.</param>
    /// <returns>A task that represents the asynchronous operation. The result contains the count of entities that satisfy the specified criteria.</returns>
    [Obsolete("Please use the specification based overload instead.")]
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, QueryOptions options = QueryOptions.None);

    /// <summary>
    /// Provides an IQueryable interface to construct and execute LINQ queries on the repository, enabling advanced query composition.
    /// </summary>
    /// <returns>An IQueryable instance representing the collection of entities in the repository, allowing for further query building.</returns>
    IQueryable<T> Query();

    /// <summary>
    /// Creates a new query specification instance for defining advanced query operations
    /// for the specified entity type.
    /// </summary>
    /// <returns>A new instance of QuerySpecification configured for use with the repository.</returns>
    QuerySpecification<T> MakeSpecification();
}