# Configuration System

## Purpose

Provides a clean abstraction over ASP.NET Core's Options pattern with validation support.

## Interfaces

- `IConfig`  
  Marker interface for configuration classes.

## Usage

```csharp
public class ConnectionStringsConfig : IConfig
{
    [Required]
    public string DefaultConnection { get; set; }
}
```

## Accessing Config in Services

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

- Apply `[Required]` and other annotations for validation.
- Configuration is validated on application boot.

