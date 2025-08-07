# Entity Validation

## Purpose

Enables automatic validation of entity data according to business rules, ensuring data integrity and consistency.

## Interfaces

- `IValidatableEntity<TKey>`  
  Extends `IEntity<TKey>` and `IValidatableObject`, and provides:
    - `IEnumerable<ValidationResult> Validate(ValidationContext validationContext)`

## Setup

To enable validation, implement the `IValidatableEntity<TKey>` interface on your entity classes:

```csharp
services.AddDbContext<DatabaseContext>((provider, options) =>
{
    options.AddBuddyInterceptors(provider);
});
```

## Usage

```csharp 
public class ComplexEntity : IValidatableEntity { 
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
```

## Notes

- The `Validate` method allows for custom validation logic beyond simple annotations.
- Validation errors are returned as `ValidationResult` objects with specific property names.
- Works with both simple and complex validation rules.