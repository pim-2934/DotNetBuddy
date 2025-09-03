using System.Linq.Expressions;
using DotNetBuddy.Domain.Enums;

namespace DotNetBuddy.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for IQueryable to perform advanced query operations such as ordering and searching.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Orders elements of a sequence based on a specified property and direction.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="queryable">The source sequence to order.</param>
    /// <param name="orderByExpression">An expression specifying the property to sort by.</param>
    /// <param name="sortDirection">The sorting direction, either ascending or descending.</param>
    /// <returns>An IQueryable representing the ordered sequence.</returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, Expression<Func<T, object>> orderByExpression,
        SortDirection sortDirection)
        where T : class
    {
        return sortDirection == SortDirection.Ascending
            ? queryable.OrderBy(orderByExpression)
            : queryable.OrderByDescending(orderByExpression);
    }

    /// <summary>
    /// Orders the elements of a sequence based on a specified property name and sort direction.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queryable sequence.</typeparam>
    /// <param name="queryable">The sequence to be ordered.</param>
    /// <param name="propertyName">The name of the property to order by.</param>
    /// <param name="sortDirection">The direction in which to order the elements.</param>
    /// <returns>An <see cref="IQueryable{T}"/> containing the ordered elements.</returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName,
        SortDirection sortDirection)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return queryable;

        var propertyInfo = typeof(T).GetProperty(propertyName,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.IgnoreCase);

        if (propertyInfo is null)
            return queryable;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var conversion = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

        return queryable.OrderBy(lambda, sortDirection);
    }

    /// <summary>
    /// Filters the given queryable collection based on the search term.
    /// Applies a search predicate to the queryable using properties marked with <c>SearchableAttribute</c>.
    /// If no search predicate can be built or if the search term is null or whitespace, the unmodified queryable is returned.
    /// </summary>
    /// <typeparam name="T">The type of the entities in the queryable collection.</typeparam>
    /// <param name="queryable">The queryable collection to filter.</param>
    /// <param name="searchTerm">The term to search for within the queryable collection.</param>
    /// <returns>The filtered queryable collection if a valid predicate is built; otherwise, the unmodified queryable.</returns>
    public static IQueryable<T> Search<T>(this IQueryable<T> queryable, string searchTerm)
        where T : class
    {
        var predicate = Utilities.SearchPredicateBuilder.Build<T>(searchTerm);

        return predicate is not null ? queryable.Where(predicate) : queryable;
    }
}