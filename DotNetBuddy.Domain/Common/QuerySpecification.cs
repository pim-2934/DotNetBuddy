using System.Linq.Expressions;
using DotNetBuddy.Domain.Enums;

namespace DotNetBuddy.Domain.Common;

/// <summary>
/// Represents a set of rules or criteria for querying data, including filtering, sorting, pagination,
/// and navigation property inclusion for entities of a specified type.
/// </summary>
/// <typeparam name="T">The type of the entity to which the specification is applied.</typeparam>
public class QuerySpecification<T>
{
    /// <summary>
    /// Defines the default number of items to include per page for paginated queries.
    /// </summary>
    /// <remarks>
    /// This value serves as the standard page size across the application for queries involving pagination.
    /// It can be explicitly overridden when a specific query requires a different page size.
    /// </remarks>
    private const int DefaultPageSize = 50;

    /// <summary>
    /// Represents the predicate expression used to filter entities in a query specification.
    /// </summary>
    /// <remarks>
    /// This property defines a LINQ expression that specifies the criteria used to filter the query results.
    /// When set, it determines which entities satisfy the condition and should be included in the query results.
    /// </remarks>
    public Expression<Func<T, bool>>? Predicate { get; private set; }

    /// <summary>
    /// Specifies the collection of navigation properties to include in the query result.
    /// </summary>
    /// <remarks>
    /// This property is used to define related entities or navigation properties
    /// that should be eagerly loaded as part of the query. By adding lambda expressions
    /// representing the desired navigation properties to the collection, it ensures
    /// that the related data is retrieved alongside the primary entity, reducing the need
    /// for subsequent queries.
    /// </remarks>
    public List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Represents the configuration options that dictate how a query is processed.
    /// </summary>
    /// <remarks>
    /// This property allows customization of query behavior by specifying options such as
    /// whether to apply tracking, enable identity resolution, bypass query filters, or use
    /// a split query. It provides greater control over the execution of queries to tailor
    /// them to specific performance or functional requirements.
    /// </remarks>
    public QueryOptions Options { get; private set; } = QueryOptions.None;

    /// <summary>
    /// Represents a collection of sorting criteria for query results, allowing the specification
    /// of multiple sort keys along with their respective sorting directions.
    /// </summary>
    /// <remarks>
    /// This property stores a list of key selectors, along with a boolean value indicating the
    /// sort direction for each key (true for ascending, false for descending). These criteria
    /// are applied sequentially to sort the query results. It is commonly used to define and
    /// manage ordered data retrieval within query specifications.
    /// </remarks>
    public List<(Expression<Func<T, object>> KeySelector, bool Ascending)> OrderBy { get; }

    /// <summary>
    /// Specifies the current page of results to retrieve in paginated queries.
    /// </summary>
    /// <remarks>
    /// This property is used in conjunction with <c>PageSize</c> to implement pagination.
    /// It determines the offset by calculating the number of items to skip before taking
    /// the specified page size. The value is set via query specification methods during
    /// query construction.
    /// </remarks>
    public int? Page { get; private set; }

    /// <summary>
    /// Defines the number of items to be included per page during paginated queries.
    /// </summary>
    /// <remarks>
    /// This property is used to control the size of pages for data pagination. It ensures
    /// that the query adheres to the specified or default page size, optimizing data retrieval
    /// and improving application performance. The value can be set explicitly through query configuration.
    /// </remarks>
    public int PageSize { get; private set; } = DefaultPageSize;

    /// <summary>
    /// Represents a base implementation for constructing specifications used to query entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity to which the specification applies.</typeparam>
    public QuerySpecification()
    {
        Includes = [];
        OrderBy = [];
    }

    /// <summary>
    /// Represents a query specification that provides filtering, sorting, includes, and pagination capabilities for querying entities.
    /// </summary>
    /// <typeparam name="T">The type of entity to which this specification applies.</typeparam>
    public QuerySpecification(Expression<Func<T, bool>> predicate) : this()
    {
        Predicate = predicate;
    }

    /// <summary>
    /// Adds an include expression to the specification for eager loading additional related entities.
    /// </summary>
    /// <param name="includeExpression">
    /// A lambda expression representing the navigation property to include in the query.
    /// </param>
    public QuerySpecification<T> AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);

        return this;
    }

    /// <summary>
    /// Adds an ordering expression to the query specification for sorting entities.
    /// </summary>
    /// <param name="orderByExpression">The expression specifying the property to sort by.</param>
    /// <param name="ascending">Indicates whether to sort in ascending order. Defaults to true if not specified.</param>
    /// <returns>The updated query specification with the added ordering expression.</returns>
    public QuerySpecification<T> AddOrderBy(Expression<Func<T, object>> orderByExpression, bool ascending = true)
    {
        OrderBy.Add((orderByExpression, ascending));

        return this;
    }

    /// <summary>
    /// Configures pagination for the query by setting the page number and the number of items per page.
    /// </summary>
    /// <param name="page">The zero-based page index to retrieve.</param>
    /// <param name="pageSize">The number of items to include per page. If not specified, a default value is used.</param>
    /// <returns>The updated query specification with pagination settings applied.</returns>
    public QuerySpecification<T> SetPage(int page, int pageSize = DefaultPageSize)
    {
        Page = page;
        PageSize = pageSize;

        return this;
    }

    /// <summary>
    /// Sets the predicate expression used to filter the query results.
    /// </summary>
    /// <param name="predicate">The predicate expression to apply as a filter.</param>
    /// <returns>An instance of <see cref="QuerySpecification{T}"/> with the specified predicate applied.</returns>
    public QuerySpecification<T> SetPredicate(Expression<Func<T, bool>> predicate)
    {
        Predicate = predicate;

        return this;
    }

    /// <summary>
    /// Sets the query options for the specification, allowing customization of how the query is processed.
    /// </summary>
    /// <param name="options">The query options to apply, specified as a combination of <see cref="QueryOptions"/> flags.</param>
    /// <returns>The current instance of <see cref="QuerySpecification{T}"/>, enabling method chaining.</returns>
    public QuerySpecification<T> SetOptions(QueryOptions options)
    {
        Options = options;

        return this;
    }
}