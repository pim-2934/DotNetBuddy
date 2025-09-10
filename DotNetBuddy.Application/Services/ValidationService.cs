using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Exceptions;

namespace DotNetBuddy.Application.Services;

/// <summary>
/// Provides an implementation of the <see cref="IValidationService"/> interface for validating source objects
/// against input objects using a registered validator service.
/// </summary>
public class ValidationService(IServiceProvider serviceProvider) : IValidationService
{
    /// <inheritdoc />
    public async IAsyncEnumerable<ValidationResult> ValidateAsync<TSource, TInput>(TSource source, TInput input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TSource : class where TInput : class
    {
        var validator = serviceProvider.GetService(typeof(IValidator<TSource, TInput>));

        if (validator is null)
            yield break;

        await foreach (var result in ((IValidator<TSource, TInput>)validator).ValidateAsync(source, input,
                           cancellationToken))
        {
            yield return result;
        }
    }

    /// <inheritdoc />
    public async Task ValidateOrThrowAsync<TSource, TInput>(TSource source, TInput input,
        CancellationToken cancellationToken = default)
        where TSource : class where TInput : class
    {
        var validationResults = new List<ValidationResult>();

        await foreach (var result in ValidateAsync(source, input, cancellationToken))
        {
            validationResults.Add(result);
        }

        if (validationResults.Count != 0)
            throw new ValidationFailedException(validationResults.Select(x => x.ErrorMessage ?? string.Empty));
    }
}