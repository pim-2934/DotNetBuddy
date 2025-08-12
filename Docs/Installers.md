# Installers

## Purpose

Supports modular service registration via `IInstaller` interface, optionally prioritized with `[InstallPriority]`.

## Interface

- `IInstaller`  
  Marker interface for installer classes.

## Attributes

- `InstallPriority`
  Allows for installer priority.  Installers with higher priority will be run first.

## Setup

```csharp
builder.Services.AddBuddy();
```
Installer classes implementing IInstaller will be discovered and executed automatically.

## Usage

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

## Notes

- Helps organize DI setup across modules.
- Use priority to control ordering (e.g., database setup before business services).
- Buddy installers run at priority 1000000000 and up.

