using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using DotNetBuddy.Application.Exceptions;
using DotNetBuddy.Domain;

namespace DotNetBuddy.Application.Services;

/// <inheritdoc />
public class ValidationService(IServiceProvider serviceProvider) : IValidationService
{
    /// <inheritdoc />
    public async IAsyncEnumerable<ValidationResult> ValidateAsync<TItem>(
        TItem item,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    ) where TItem : class
    {
        var validator = serviceProvider.GetService(typeof(IValidator<TItem>));

        if (validator is null)
            yield break;

        await foreach (var result in ((IValidator<TItem>)validator).ValidateAsync(item, cancellationToken))
        {
            yield return result;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ValidationResult> ValidateAsync<TItem, TInput>(
        TItem item,
        TInput input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    ) where TItem : class where TInput : class
    {
        var validator = serviceProvider.GetService(typeof(IValidator<TItem, TInput>));

        if (validator is null)
            yield break;

        await foreach (var result in ((IValidator<TItem, TInput>)validator).ValidateAsync(item, input,
                           cancellationToken))
        {
            yield return result;
        }
    }

    /// <inheritdoc />
    public async Task ValidateOrThrowAsync<TItem>(TItem item, CancellationToken cancellationToken = default)
        where TItem : class
    {
        var validationResults = new List<ValidationResult>();

        await foreach (var result in ValidateAsync(item, cancellationToken))
        {
            validationResults.Add(result);
        }

        if (validationResults.Count != 0)
            throw new ValidationFailedException(validationResults.Select(x => x.ErrorMessage ?? string.Empty));
    }

    /// <inheritdoc />
    public async Task ValidateOrThrowAsync<TItem, TInput>(
        TItem item,
        TInput input,
        CancellationToken cancellationToken = default
    ) where TItem : class where TInput : class
    {
        var validationResults = new List<ValidationResult>();

        await foreach (var result in ValidateAsync(item, input, cancellationToken))
        {
            validationResults.Add(result);
        }

        if (validationResults.Count != 0)
            throw new ValidationFailedException(validationResults.Select(x => x.ErrorMessage ?? string.Empty));
    }
}