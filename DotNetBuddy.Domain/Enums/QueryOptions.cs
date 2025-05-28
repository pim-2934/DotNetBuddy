namespace DotNetBuddy.Domain.Enums;

/// <summary>
/// Represents the available options that can modify the behavior of a query when interacting with the data layer.
/// These options are designed to provide control over query execution and behavior in specific scenarios.
/// </summary>
[Flags]
public enum QueryOptions
{
    /// <summary>
    /// Denotes the default option where no specific query behavior modifications are applied.
    /// This implies that the query operates under its standard execution settings without any additional flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies that entities retrieved by the query should not be tracked in the current context.
    /// This option is useful for read-only operations where changes to the entities are not required
    /// to be tracked, improving performance by avoiding overhead associated with tracking.
    /// </summary>
    AsNoTracking = 1 << 0,

    /// <summary>
    /// Specifies that the query is executed without tracking entities in the change tracker,
    /// but retains identity resolution. This ensures that the same entity is returned across the query
    /// results when it appears multiple times, maintaining consistency without enabling full tracking.
    /// Useful for read-focused operations where entity identity is important, but persistence tracking is not required.
    /// </summary>
    AsNoTrackingWithIdentityResolution = 1 << 1,

    /// <summary>
    /// Indicates that query filters, usually defined globally in the context, should be ignored during query execution.
    /// This allows the query to retrieve all data, bypassing any filters set for purposes such as soft deletion or multi-tenancy.
    /// </summary>
    IgnoreQueryFilters = 1 << 2,

    /// <summary>
    /// Indicates that queries should be split into multiple SQL queries for collections,
    /// avoiding the default behavior of generating a single SQL query. This is useful
    /// for optimizing performance in scenarios where large collections might result in
    /// inefficient or complex query execution.
    /// </summary>
    UseSplitQuery = 1 << 3
}
