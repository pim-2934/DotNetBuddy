using System.Linq.Expressions;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for IQueryable to enhance query functionality,
/// including support for query includes and applying specific query options.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies the specified query includes to the provided queryable data source.
    /// This allows for the inclusion of related entities in the query result.
    /// </summary>
    /// <typeparam name="T">The type of entity in the query.</typeparam>
    /// <param name="query">The queryable data source to which includes will be applied.</param>
    /// <param name="includes">An array of expressions specifying the related entities to include in the query.</param>
    /// <returns>A queryable data source with the specified includes applied.</returns>
    public static IQueryable<T> ApplyQueryIncludes<T>(
        this IQueryable<T> query,
        params Expression<Func<T, object>>[] includes)
        where T : class
    {
        if (includes.Length == 0)
            return query;

        var includePaths = new Dictionary<string, string>();
        foreach (var include in includes)
        {
            var path = ExpressionPathVisitor.GetPropertyPath(include);
            if (!string.IsNullOrEmpty(path))
            {
                includePaths[path] = path;
            }
        }

        var orderedPaths = includePaths.Values
            .OrderBy(p => p.Length)
            .ToList();

        return orderedPaths
            .Where(path => !orderedPaths.Any(p => p != path && path.StartsWith(p + ".")))
            .Aggregate(query, (current, path) => current.Include(path));
    }

    /// <summary>
    /// Applies specific query options to an IQueryable instance, modifying its behavior
    /// based on the provided flags for tracking, query filters, and execution strategy.
    /// </summary>
    /// <typeparam name="T">The type of entity in the query.</typeparam>
    /// <param name="query">The query to which the options will be applied.</param>
    /// <param name="options">The options defining how the query should be modified.</param>
    /// <returns>The modified query with the specified options applied.</returns>
    public static IQueryable<T> ApplyQueryOptions<T>(this IQueryable<T> query, QueryOptions options)
        where T : class
    {
        if (options.HasFlag(QueryOptions.AsNoTracking) &&
            options.HasFlag(QueryOptions.AsNoTrackingWithIdentityResolution))
        {
            throw new BuddyException
            (
                "Invalid query options.",
                "Cannot specify both AsNoTracking and AsNoTrackingWithIdentityResolution."
            );
        }

        if (options.HasFlag(QueryOptions.AsNoTracking))
        {
            query = query.AsNoTracking();
        }

        if (options.HasFlag(QueryOptions.AsNoTrackingWithIdentityResolution))
        {
            query = query.AsNoTrackingWithIdentityResolution();
        }

        if (options.HasFlag(QueryOptions.IgnoreQueryFilters))
        {
            query = query.IgnoreQueryFilters();
        }

        if (options.HasFlag(QueryOptions.UseSplitQuery))
        {
            query = query.AsSplitQuery();
        }

        return query;
    }
}