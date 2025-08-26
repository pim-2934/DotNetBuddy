namespace DotNetBuddy.Domain.Enums;

/// <summary>
/// Represents the direction in which a sorting operation should be performed.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Represents an ascending sort direction for ordering data.
    /// </summary>
    /// <remarks>
    /// When this value is used, elements are sorted in increasing order (e.g., A-Z for strings or 1-10 for numbers).
    /// It is primarily used in query specifications and other operations requiring ordered data.
    /// </remarks>
    Ascending,

    /// <summary>
    /// Represents a descending sort direction used to arrange items from highest to lowest or from most recent to oldest.
    /// </summary>
    Descending
}