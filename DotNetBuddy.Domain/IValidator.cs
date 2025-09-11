using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Domain;

/// <summary>
/// Provides a mechanism for validating objects of the specified type.
/// </summary>
/// <typeparam name="TItem">The type of object to validate. Must be a reference type.</typeparam>
public interface IValidator<in TItem> where TItem : class
{
    /// <summary>
    /// Validates the provided source object asynchronously using a registered validator service.
    /// </summary>
    /// <param name="item">The source object to be validated.</param>
    /// <param name="cancellationToken">An optional token to observe for cancellation requests.</param>
    /// <returns>A collection of <see cref="ValidationResult"/> objects that represent the results of validation.
    /// If no validator is registered, or if no validation errors occur, an empty collection is returned.</returns>
    IAsyncEnumerable<ValidationResult> ValidateAsync(TItem item, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines an interface for validating a source object against a provided input object asynchronously.
/// </summary>
/// <typeparam name="TItem">The type of the source object to be validated. Must be a reference type.</typeparam>
/// <typeparam name="TInput">The type of the input object used for validation. Must be a reference type.</typeparam>
public interface IValidator<in TItem, in TInput> where TItem : class where TInput : class
{
    /// <summary>
    /// Validates the provided source object against input parameters using the appropriate validator service implementation.
    /// </summary>
    /// <param name="item">The source object to be validated.</param>
    /// <param name="input">The input object containing new data.</param>
    /// <param name="cancellationToken">An optional token used to observe cancellation requests.</param>
    /// <returns>A collection of <see cref="ValidationResult"/> objects representing the validation results.
    /// Returns an empty collection if no validator is resolved or no validation errors are found.</returns>
    IAsyncEnumerable<ValidationResult> ValidateAsync(
        TItem item,
        TInput input,
        CancellationToken cancellationToken = default
    );
}