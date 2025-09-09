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
    /// Validates the provided source object against the input parameters using a registered validator service.
    /// </summary>
    /// <param name="source">The source object to be validated.</param>
    /// <param name="input">The input object containing the validation criteria.</param>
    /// <param name="cancellationToken">An optional token to observe for cancellation requests.</param>
    /// <typeparam name="TSource">The type of the source object to validate, must be a reference type.</typeparam>
    /// <typeparam name="TInput">The type of the input object used for validation, must be a reference type.</typeparam>
    /// <returns>A collection of <see cref="ValidationResult"/> objects that represent the results of validation.
    /// If no validator is registered, or if no validation errors occur, an empty collection is returned.</returns>
    IAsyncEnumerable<ValidationResult> ValidateAsync(TSource source, TInput input,
        CancellationToken cancellationToken = default);
}