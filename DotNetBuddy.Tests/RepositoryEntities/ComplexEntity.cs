using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;

namespace DotNetBuddy.Tests.RepositoryEntities;

public class ComplexEntity : IValidatableEntity<Guid>
{
    public Guid Id { get; set; }
    public int Foo { get; set; }
    public int Bar { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Bar > Foo)
        {
            yield return new ValidationResult("Bar is not allowed to be greater than Foo.", [nameof(Bar)]);
        }
    }
}