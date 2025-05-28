namespace DotNetBuddy.Domain;

/// <summary>
/// Represents an entity that supports soft deletion by maintaining a nullable timestamp for the deletion date.
/// </summary>
/// <typeparam name="TKey">
/// The type of the unique identifier for the entity.
/// </typeparam>
public interface ISoftDeletableEntity<TKey> : IEntity<TKey>
{
    /// <summary>
    /// Specifies the date and time when the entity was soft-deleted.
    /// </summary>
    /// <remarks>
    /// A null value indicates that the entity is not deleted. This property is typically used
    /// in conjunction with soft-delete mechanisms to logically delete an entity without
    /// removing it from the database.
    /// </remarks>
    public DateTime? DeletedAt { get; set; }
}