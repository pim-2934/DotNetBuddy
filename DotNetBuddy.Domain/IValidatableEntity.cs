using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Domain;

/// <summary>
/// Provides a contract for an entity that supports validation rules
/// and has a unique identifier. Extends the functionality of both IEntity
/// for identification and IValidatableObject for validation.
/// </summary>
/// <typeparam name="TKey">
/// The type of the unique identifier for the entity.
/// </typeparam>
public interface IValidatableEntity<TKey> : IEntity<TKey>, IValidatableObject;