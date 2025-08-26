using DotNetBuddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for IQueryable to enhance query functionality,
/// including support for query includes and applying specific query options.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies specific query options to an IQueryable instance, modifying its behavior
    /// based on the provided flags for tracking, query filters, and execution strategy.
    /// </summary>
    /// <typeparam name="T">The type of entity in the query.</typeparam>
    /// <param name="query">The query to which the options will be applied.</param>
    /// <param name="option">The options defining how the query should be modified.</param>
    /// <returns>The modified query with the specified options applied.</returns>
    public static IQueryable<T> ApplyQueryOptions<T>(this IQueryable<T> query, QueryOptions option)
        where T : class
    {
        if (option.HasFlag(QueryOptions.WithSoftDeleted))
        {
            query = query.IgnoreQueryFilters();
        }

        return query;
    }
}