namespace BuddyDotNet.Enums;

/// <summary>
/// Represents options that can modify the behavior of query execution in a repository.
/// These options influence tracking behavior, query filter application, or query splitting.
/// </summary>
[Flags]
public enum QueryOptions
{
    /// <summary>
    /// Specifies that no query options are applied, meaning the query is executed with the default behavior.
    /// This is used to indicate the absence of any modifications to the query execution process.
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies that the query should be executed without tracking changes to the resulting entities.
    /// This allows for more efficient read-only operations, as entities are not tracked by the context
    /// for change detection or updates.
    /// </summary>
    AsNoTracking = 1 << 0,

    /// <summary>
    /// Specifies that the query should be executed without tracking changes to the resulting entities,
    /// but ensures identity resolution is applied. This enables entities with the same identity to be
    /// resolved to the same instance during the query execution, while maintaining a no-tracking context.
    /// </summary>
    AsNoTrackingWithIdentityResolution = 1 << 1,

    /// <summary>
    /// Specifies that the query should ignore any defined query filters, bypassing additional filtering logic
    /// such as tenant-based, soft-delete, or other global query constraints.
    /// </summary>
    IgnoreQueryFilters = 1 << 2,

    /// <summary>
    /// Specifies that the query should be executed as a split query, which separates data loading into multiple queries
    /// to optimize performance and reduce server-side data processing overhead.
    /// </summary>
    UseSplitQuery = 1 << 3
}
