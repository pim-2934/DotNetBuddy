namespace DotNetBuddy.Domain.Constants;

/// <summary>
/// Provides a set of constant string keys to be used in the validation context.
/// </summary>
public static class ValidationContextKeys
{
    /// <summary>
    /// Represents the key used to identify the state of an entity in validation context,
    /// typically used for tracking and managing entity state during validation operations.
    /// </summary>
    public const string EntityState = "EntityState";

    /// <summary>
    /// Represents the key used to store or retrieve original values of an entity
    /// within a validation context. This constant is typically utilized to track
    /// and manage the original state of data prior to modifications.
    /// </summary>
    public const string OriginalValues = "OriginalValues";
}