using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Domain;

/// <summary>
/// Defines a contract for an entity that combines unique identification and
/// data validation capabilities. This interface integrates IEntity for unique
/// identity representation and IValidatableObject for implementation of custom
/// validation logic.
/// </summary>
/// <typeparam name="TKey">
/// Specifies the type of the unique identifier used in the entity.
/// </typeparam>
public interface IValidatableEntity<TKey> : IEntity<TKey>, IValidatableObject;