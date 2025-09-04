using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Domain;

/// <summary>
/// Represents a generic interface for validating a source object against a given input object.
/// </summary>
/// <typeparam name="TSource">The type of the source object to be validated. Must be a reference type.</typeparam>
/// <typeparam name="TInput">The type of the input object used for validation. Must be a reference type.</typeparam>
public interface IValidator<in TSource, in TInput> where TSource : class where TInput : class
{
    /// <summary>
    /// Validates the provided source object against the input parameters using a registered validator.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object being validated.</typeparam>
    /// <typeparam name="TInput">The type of the input parameters used for validation.</typeparam>
    /// <param name="source">The source object to be validated.</param>
    /// <param name="input">The input parameters against which the source object is validated.</param>
    /// <returns>A collection of <see cref="ValidationResult"/> objects containing validation results.
    /// Returns an empty collection if no validator is registered or no validation errors occur.</returns>
    IEnumerable<ValidationResult> Validate(TSource source, TInput input);
}