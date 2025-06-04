namespace DotNetBuddy.Domain.Common;

/// <inheritdoc />
public class EntityPagedResult<T, TKey>(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    : IEntityPagedResult<T, TKey>
    where T : IEntity<TKey>
{
    /// <inheritdoc />
    public IReadOnlyList<T> Items { get; } = items;

    /// <inheritdoc />
    public int PageNumber { get; } = pageNumber;

    /// <inheritdoc />
    public int PageSize { get; } = pageSize;

    /// <inheritdoc />
    public int TotalCount { get; } = count;

    /// <inheritdoc />
    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);

    /// <inheritdoc />
    public bool HasPreviousPage => PageNumber > 1;

    /// <inheritdoc />
    public bool HasNextPage => PageNumber < TotalPages;
}