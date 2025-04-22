namespace DotNetBuddy;

/// <summary>
/// Defines a contract for entities that maintain audit information, including creation and update timestamps.
/// </summary>
public interface IAuditableEntity : IEntity
{
    /// <summary>
    /// Gets or sets the timestamp indicating when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp indicating the last time the entity was modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}