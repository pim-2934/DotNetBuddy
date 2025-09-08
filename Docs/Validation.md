# Validation (Request/Entity Pair)

## Purpose

Provide a simple, explicit, and decoupled validation mechanism by defining validators for a specific source object (typically an entity/read model) and an input object (typically a DTO/command). Validators are discovered via DI and can be invoked through a centralized `IValidationService`.

## Core Interfaces

- `IValidator<TSource, TInput>`
  - Defines `IEnumerable<ValidationResult> Validate(TSource source, TInput input)`.
  - Return an empty sequence when the input is valid.
- `IValidationService`
  - `IEnumerable<ValidationResult> Validate<TSource, TInput>(TSource source, TInput input)`
  - `void ValidateOrThrow<TSource, TInput>(TSource source, TInput input)` which throws `DotNetBuddy.Domain.Exceptions.ValidationException` if any errors exist.

## Installation / Setup

```csharp
services.AddBuddy();
```

If some assemblies containing validators are not loaded yet at the time of registration, you can pass their AssemblyName to preload them:

```csharp
services.AddBuddy(typeof(SomeProjectMarker).Assembly.GetName());
```

## Creating a Validator

Implement `IValidator<TSource, TInput>` for the pair you need to validate.

```csharp
using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using DotNetBuddy.Example.Contracts;
using DotNetBuddy.Example.Entities;

namespace DotNetBuddy.Example.Validators;

public class WeatherForecastUpdateValidator : IValidator<WeatherForecast, WeatherForecastUpdateDto>
{
    public IEnumerable<ValidationResult> Validate(WeatherForecast source, WeatherForecastUpdateDto input)
    {
        if (source.TemperatureC < input.TemperatureC)
            yield return new ValidationResult("Temperature cannot be greater than the original value.");

        if (source.Summary == input.Summary)
            yield return new ValidationResult("Summary cannot be the same as the original value.");
    }
}
```

You can optionally supply member names when relevant to tie an error to a specific field:

```csharp
yield return new ValidationResult("Bar is not allowed to be greater than Foo.", new[] { nameof(input.BelowBaseValue) });
```

## Using the Validation Service

Inject `IValidationService` where needed (e.g., in controllers, handlers, or services) and call `Validate` or `ValidateOrThrow`.

```csharp
public SomeService(IValidationService validationService)
{
    public async Task DoSomethingAsync(object source, object input)
    {
        // throws ValidationException if any errors exist.
        var results = await _validationService.ValidateOrThrow(source, input);
    }
}
```

### Behavior when no validator is registered
- `Validate` returns an empty sequence.
- `ValidateOrThrow` does not throw.
This allows you to add validators incrementally without breaking flows.

## Notes & Tips

- Keep validators focused on business rules that involve both the current state (source) and the desired change (input).
- Validators are plain classes and do not depend on EF or a specific persistence mechanism.
- Use `ValidateOrThrow` to centralize error handling.
- You can have multiple validators across different request/entity pairs. The DI system resolves the exact `IValidator<TSource, TInput>` matching the pair you validate.
