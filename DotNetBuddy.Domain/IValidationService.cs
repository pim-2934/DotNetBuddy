using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain.Exceptions;

namespace DotNetBuddy.Domain;

/// <summary>
/// Represents a service for performing validation operations on objects, optionally against specified inputs.
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates the specified item of type <typeparamref name="TItem"/> asynchronously and returns a collection of <see cref="ValidationResult"/> containing validation errors, if any.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to validate. This must be a reference type.</typeparam>
    /// <param name="item">The object to validate.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to observe cancellation requests.</param>
    /// <returns>An asynchronous sequence of <see cref="ValidationResult"/> detailing any validation errors found.</returns>
    IAsyncEnumerable<ValidationResult> ValidateAsync<TItem>(
        TItem item,
        CancellationToken cancellationToken = default
    ) where TItem : class;

    /// <summary>
    /// Asynchronously validates the provided source and input objects using registered validators.
    /// </summary>
    /// <typeparam name="TItem">The type of the source object to validate. Must be a reference type.</typeparam>
    /// <typeparam name="TInput">The type of the input object to validate against the source. Must be a reference type.</typeparam>
    /// <param name="item">The source object to validate.</param>
    /// <param name="input">The input object used for validation.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>An asynchronous stream of <see cref="ValidationResult"/> instances representing the validation failures.</returns>
    IAsyncEnumerable<ValidationResult> ValidateAsync<TItem, TInput>(
        TItem item,
        TInput input,
        CancellationToken cancellationToken = default
    ) where TItem : class where TInput : class;

    /// <summary>
    /// Validates the specified item and throws an exception if validation fails.
    /// </summary>
    /// <typeparam name="TItem">The type of the object to validate.</typeparam>
    /// <param name="item">The instance of the object to validate.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    /// <exception cref="ValidationException">Thrown if the validation fails for the specified item.</exception>
    Task ValidateOrThrowAsync<TItem>(
        TItem item,
        CancellationToken cancellationToken = default
    ) where TItem : class;

    /// <summary>
    /// Validates the specified source object using the provided input object, and throws a <see cref="ValidationFailedException"/>
    /// if any validation errors are encountered.
    /// </summary>
    /// <typeparam name="TItem">The type of the source object to validate.</typeparam>
    /// <typeparam name="TInput">The type of the input object used for validation.</typeparam>
    /// <param name="item">The source object to be validated.</param>
    /// <param name="input">The input object against which the source object is validated.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the validation process.</param>
    /// <returns>A task that represents the asynchronous validation operation.</returns>
    /// <exception cref="ValidationFailedException">Thrown when validation fails and errors are present.</exception>
    Task ValidateOrThrowAsync<TItem, TInput>(
        TItem item,
        TInput input,
        CancellationToken cancellationToken = default
    ) where TItem : class where TInput : class;
}