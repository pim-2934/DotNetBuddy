using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using ValidationException = DotNetBuddy.Domain.Exceptions.ValidationException;

namespace DotNetBuddy.Application.Services;

/// <summary>
/// Provides an implementation of the <see cref="IValidationService"/> interface for validating source objects
/// against input objects using a registered validator service.
/// </summary>
public class ValidationService(IServiceProvider serviceProvider) : IValidationService
{
    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate<TSource, TInput>(TSource source, TInput input)
        where TSource : class where TInput : class
    {
        var validator = serviceProvider.GetService(typeof(IValidator<TSource, TInput>));

        return validator is null ? [] : ((IValidator<TSource, TInput>)validator).Validate(source, input);
    }

    /// <inheritdoc />
    public void ValidateOrThrow<TSource, TInput>(TSource source, TInput input)
        where TSource : class where TInput : class
    {
        var validationResults = Validate(source, input).ToList();

        if (validationResults.Count != 0)
            throw new ValidationException(string.Join(",", validationResults.Select(x => x.ErrorMessage)));
    }
}