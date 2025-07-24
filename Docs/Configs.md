# Configuration System

## Purpose

Provides a clean abstraction over ASP.NET Core's Options pattern, with built-in support for validation.
Simplifies registration and ensures configuration values are validated at application startup.

## Interfaces

- `IConfig`  
  Marker interface for configuration classes.

## Setup

```csharp
builder.Services.AddBuddy<DatabaseContext>();
```
Configuration classes implementing IConfig will be discovered and registered automatically.

## Usage

Define a configuration class:
```csharp
public class ConnectionStringsConfig : IConfig
{
    [Required]
    public string DefaultConnection { get; set; }
}
```

Inject and access configuration:
```csharp
public class SomeService
{
    private readonly ConnectionStringsConfig _config;

    public SomeService(IOptions<ConnectionStringsConfig> options)
    {
        _config = options.Value;
    }
}
```

## Notes

- Use `[Required]` and other DataAnnotations to enforce validation.
- Configuration is validated once at application startup — invalid configuration will cause the application to fail fast.
- Supports standard ASP.NET Core configuration sources (appsettings.json, environment variables, etc.).



