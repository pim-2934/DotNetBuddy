# Audit System

## Purpose

Enables automatic tracking of creation and modification timestamps on entities for auditing purposes.

## Interfaces

- `IAuditableEntity`  
  Extends `IEntity`, and provides:
    - `DateTime CreatedAt`
    - `DateTime UpdatedAt`

## Setup

To enable auditing, register `BuddyInterceptors` in your `DbContext` setup:

```csharp
services.AddDbContext<DatabaseContext>((provider, options) =>
{
    options.AddBuddyInterceptors(provider);
});
```

## Entity Implementation

```csharp
public class MyEntity : IAuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Notes

- `AddBuddyInterceptors` ensures timestamps are automatically handled.
- Works with both in-memory and real databases.

