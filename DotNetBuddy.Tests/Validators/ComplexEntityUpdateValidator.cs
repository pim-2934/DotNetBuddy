using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using DotNetBuddy.Tests.Entities;

namespace DotNetBuddy.Tests.Validators;

public class ComplexEntityUpdateValidator : IValidator<ComplexEntity, ComplexEntity>
{
    public IEnumerable<ValidationResult> Validate(ComplexEntity source, ComplexEntity input)
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
    }
}