using System.Linq.Expressions;
using DotNetBuddy.Domain.Common;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Domain.Exceptions;
using DotNetBuddy.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for IQueryable to enhance query functionality,
/// including support for query includes and applying specific query options.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies the specified query specification to the provided IQueryable data source.
    /// This includes filtering, ordering, pagination, and the inclusion of related entities.
    /// </summary>
    /// <typeparam name="T">The type of the entities in the data source.</typeparam>
    /// <param name="query">The IQueryable data source to which the specification will be applied.</param>
    /// <param name="spec">The specification containing the rules and options to apply to the query.</param>
    /// <param name="applyPaging">Indicates whether paging should be applied. If set to true, pagination logic will be executed.</param>
    /// <returns>The IQueryable data source modified according to the supplied specification rules.</returns>
    public static IQueryable<T> ApplySpecification<T>(
        this IQueryable<T> query,
        QuerySpecification<T> spec,
        bool applyPaging = true) where T : class
    {
        if (spec.Predicate is not null)
            query = query.Where(spec.Predicate);

        query = query.ApplyQueryOptions(spec.Options);
        query = query.ApplyQueryIncludes(spec.Includes.ToArray());

        if (spec.OrderBy.Count > 0)
        {
            var isFirst = true;
            foreach (var (keySelector, ascending) in spec.OrderBy)
            {
                if (isFirst)
                {
                    query = ascending
                        ? query.OrderBy(keySelector)
                        : query.OrderByDescending(keySelector);
                    isFirst = false;
                }
                else
                {
                    query = ascending
                        ? ((IOrderedQueryable<T>)query).ThenBy(keySelector)
                        : ((IOrderedQueryable<T>)query).ThenByDescending(keySelector);
                }
            }
        }

        if (applyPaging && spec.Page is not null)
            query = query.Skip((spec.Page.Value - 1) * spec.PageSize).Take(spec.PageSize);

        return query;
    }

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
            throw new BuddyException("Cannot specify both AsNoTracking and AsNoTrackingWithIdentityResolution.");
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