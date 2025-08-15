using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Tests.RepositoryEntities;

public class ComplexEntity : IValidatableEntity<Guid>
{
    public Guid Id { get; set; }
    public int BaseValue { get; set; }
    public int BelowBaseValue { get; set; }
    public int UnchangeableValue { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (validationContext.Items.TryGetValue(ValidationContextKeys.EntityState, out var entityState) &&
            entityState is EntityState.Modified)
        {
            if (validationContext.Items.TryGetValue(ValidationContextKeys.OriginalValues, out var originalValues) &&
                originalValues is Dictionary<string, object?> originalValuesDict)
            {
                if (originalValuesDict.TryGetValue(nameof(UnchangeableValue), out var originalValue) &&
                    originalValue is int originalIntValue &&
                    originalIntValue != UnchangeableValue)
                {
                    yield return new ValidationResult(
                        "UnchangeableValue cannot be modified once set.",
                        [nameof(UnchangeableValue)]
                    );
                }
            }
        }

        if (BelowBaseValue > BaseValue)
        {
            yield return new ValidationResult("Bar is not allowed to be greater than Foo.", [nameof(BelowBaseValue)]);
        }
    }
}