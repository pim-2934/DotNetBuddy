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
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous stream of <see cref="ValidationResult"/> objects containing validation results. Returns an empty stream if no validator is registered or if no validation errors are found.</returns>
    IAsyncEnumerable<ValidationResult> ValidateAsync<TSource, TInput>(TSource source, TInput input,
        CancellationToken cancellationToken = default)
        where TSource : class where TInput : class;

    /// <summary>
    /// Validates the specified source object against the given input object using a registered validator service,
    /// and throws a validation exception if validation fails.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object to be validated. Must be a reference type.</typeparam>
    /// <typeparam name="TInput">The type of the input object used for validation. Must be a reference type.</typeparam>
    /// <param name="source">The source object to validate.</param>
    /// <param name="input">The input object used for validation.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. If any validation errors are found, an exception is thrown, and this task does not complete successfully.</returns>
    Task ValidateOrThrowAsync<TSource, TInput>(TSource source, TInput input,
        CancellationToken cancellationToken = default) where TSource : class where TInput : class;
}