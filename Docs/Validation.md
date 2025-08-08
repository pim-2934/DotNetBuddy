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
public class ComplexEntity : IValidatableEntity<Guid>
{
    public Guid Id { get; set; }
    public int BaseValue { get; set; }
    public int BelowBaseValue { get; set; }
    public int UnchangeableValue { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Basic validation - ensure BelowBaseValue doesn't exceed BaseValue
        if (BelowBaseValue > BaseValue)
        {
            yield return new ValidationResult("BelowBaseValue is not allowed to be greater than BaseValue.", [nameof(BelowBaseValue)]);
        }
        
        // You can access entity state and original values from the validation context
        if (validationContext.Items.TryGetValue(ValidationContextKeys.EntityState, out var entityState) &&
            entityState is EntityState.Modified)
        {
            // Access the entity state (Added, Modified, Deleted, etc.)
            // This helps customize validation based on the current state of the entity
            if (validationContext.Items.TryGetValue(ValidationContextKeys.OriginalValues, out var originalValues) &&
                originalValues is Dictionary<string, object?> originalValuesDict)
            {
                // Access original property values before changes
                // Useful for validation rules that depend on what changed
                
                // Example: Prevent UnchangeableValue from being modified once set
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
    }
}
```

## Additional Context Information
The validation context provides additional information that can be useful for complex validation scenarios:
- **EntityState**: Available via `ValidationContextKeys.EntityState`
  - Contains the current state of the entity (Added, Modified, Deleted, etc.)
  - Useful for applying different validation rules based on the operation being performed

- **OriginalValues**: Available via `ValidationContextKeys.OriginalValues`
  - Contains the original property values before any changes were made
  - Enables validation based on what has changed, rather than just current values
  - Particularly useful for ensuring valid transitions between states

## Notes

- The `Validate` method allows for custom validation logic beyond simple annotations.
- Validation errors are returned as `ValidationResult` objects with specific property names.
- Works with both simple and complex validation rules.
- Context-aware validation allows for more sophisticated business rules.
