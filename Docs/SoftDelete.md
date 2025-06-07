# Soft Delete System

## Purpose

Enables entities to be excluded from standard queries rather than being permanently deleted from the database. This
allows entities to be "soft deleted," preserving them for auditing, recovery, or historical reference.

## Interfaces

- `ISoftDeletableEntity<TKey>`  
  Extends `IEntity<TKey>`, and provides:
    - `DateTime? DeletedAt`

## Setup

To enable soft delete filtering, call `ApplySoftDeleteQueryFilters` during your `DbContext` model configuration:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder) 
{
    base.OnModelCreating(modelBuilder);
    
    modelBuilder.ApplySoftDeleteQueryFilters(); 
}
```

## Entity Implementation

```csharp
public class MyEntity : ISoftDeletableEntity 
{
    public Guid Id { get; set; } 
    public DateTime? DeletedAt { get; set; }
}
```

## Accessing Soft-Deleted Entities

To query soft deleted entities (those with a non-null `DeletedAt`), use the `QueryOptions.IgnoreQueryFilters` flag when
calling the repository methods. For example:

```csharp
// Get only soft-deleted entities 
var deleted = await repository.GetRangeAsync( e => e.DeletedAt != null, QueryOptions.IgnoreQueryFilters);

// Get all entities, both active and soft-deleted 
var all = await repository.GetRangeAsync(QueryOptions.IgnoreQueryFilters);
```

## Notes

- Entities with a `DeletedAt` value of `null` are considered active and included in queries by default.
- Entities with a non-`null` `DeletedAt` value are excluded from query results unless explicitly included.
- The soft delete pattern preserves data integrity and allows for entity recovery.
- Ensure to handle soft deletion in your application logic (e.g., by setting `DeletedAt` instead of removing records).