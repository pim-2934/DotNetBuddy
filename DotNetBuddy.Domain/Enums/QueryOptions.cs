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
    /// Indicates that the query should include entities that have been soft deleted.
    /// When this flag is set, the query will return both active and soft-deleted records,
    /// bypassing the default behavior of filtering out soft-deleted entities.
    /// </summary>
    WithSoftDeleted = 1
}