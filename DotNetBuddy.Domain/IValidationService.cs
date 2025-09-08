using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Domain;

/// <summary>
/// Provides methods for validating a source object against a given input object.
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates the specified source object against the given input object by using a registered validator service.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object to be validated. Must be a reference type.</typeparam>
    /// <typeparam name="TInput">The type of the input object used for validation. Must be a reference type.</typeparam>
    /// <param name="source">The source object to validate.</param>
    /// <param name="input">The input object used for validation.</param>
    /// <returns>A collection of <see cref="ValidationResult"/> objects containing validation results. Returns an empty collection if no validator is registered or if no validation errors are found.</returns>
    IEnumerable<ValidationResult> Validate<TSource, TInput>(TSource source, TInput input)
        where TSource : class where TInput : class;

    /// <summary>
    /// Validates the specified source object against the given input object using a registered validator service,
    /// and throws a validation exception if validation fails.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object to be validated. Must be a reference type.</typeparam>
    /// <typeparam name="TInput">The type of the input object used for validation. Must be a reference type.</typeparam>
    /// <param name="source">The source object to validate.</param>
    /// <param name="input">The input object used for validation.</param>
    /// <returns>A collection of <see cref="ValidationResult"/> objects containing validation results. If any validation errors are found, an exception is thrown, and this method does not return normally.</returns>
    void ValidateOrThrow<TSource, TInput>(TSource source, TInput input) where TSource : class where TInput : class;
}