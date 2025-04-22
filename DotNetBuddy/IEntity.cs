using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy;

/// <summary>
/// Represents a base contract for entities with a unique identifier.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for an entity.
    /// </summary>
    [Key] public Guid Id { get; set; }
}