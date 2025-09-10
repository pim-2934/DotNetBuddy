using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using DotNetBuddy.Domain;
using DotNetBuddy.Tests.Entities;

namespace DotNetBuddy.Tests.Validators;

public class ComplexEntityUpdateValidator : IValidator<ComplexEntity, ComplexEntity>
{
    public async IAsyncEnumerable<ValidationResult> ValidateAsync(ComplexEntity source, ComplexEntity input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (source.UnchangeableValue != input.UnchangeableValue)
        {
            yield return new ValidationResult(
                "UnchangeableValue cannot be modified once set.",
                [nameof(source.UnchangeableValue)]
            );
        }

        if (input.BelowBaseValue > input.BaseValue)
        {
            yield return new ValidationResult("Bar is not allowed to be greater than Foo.",
                [nameof(input.BelowBaseValue)]);
        }

        await Task.CompletedTask;
    }
}