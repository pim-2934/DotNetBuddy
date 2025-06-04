namespace DotNetBuddy.Domain;

/// <summary>
/// Defines the contract for a paginated result set of entities.
/// </summary>
/// <typeparam name="T">The type of entity in the paginated collection.</typeparam>
/// <typeparam name="TKey">The type of the unique identifier used for the entity.</typeparam>
public interface IEntityPagedResult<out T, TKey> where T : IEntity<TKey>
{
    /// <summary>
    /// Gets the entities on the current page.
    /// </summary>
    IReadOnlyList<T> Items { get; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Gets the size of each page.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Gets the total number of entities across all pages.
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    bool HasNextPage { get; }
}