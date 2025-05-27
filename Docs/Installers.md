# Installers

## Purpose

Supports modular service registration via `IInstaller` interface, optionally prioritized with `[InstallPriority]`.

## Interface

- `IInstaller`  
  Marker interface for installer classes.

## Example

```csharp
[InstallPriority(200000)]
public class DependencyInstaller : IInstaller
{
    public void Install(IServiceCollection services)
    {
        services.AddScoped<ISampleService, SampleService>();
    }
}
```

## Prioritized Setup

Installers are executed in order of their `[InstallPriority]`.

## Notes

- Helps organize DI setup across modules.
- Use priority to control ordering (e.g., database setup before business services).

