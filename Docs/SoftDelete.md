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

## Usage

```csharp
public class MyEntity : ISoftDeletableEntity 
{
    public Guid Id { get; set; } 
    public DateTime? DeletedAt { get; set; }
}
```

## Example (Accessing Soft-Deleted Entities)

To query soft-deleted entities (those with a non-null `DeletedAt`), use `QueryOptions.WithSoftDeleted` when starting your query via the repository, or apply options to an existing `IQueryable<T>` using the extension methods.

```csharp
// Using repository.MakeQuery with options
var queryDeleted = repository
    .MakeQuery(QueryOptions.WithSoftDeleted)
    .Where(e => e.DeletedAt != null);
var deleted = await repository.GetRangeAsync(queryDeleted);

// Get all entities, both active and soft-deleted
var queryAll = repository.MakeQuery(QueryOptions.WithSoftDeleted);
var all = await repository.GetRangeAsync(queryAll);

// Alternatively, apply options to an existing IQueryable<T>
var baseQuery = repository.MakeQuery();
var all2 = await repository.GetRangeAsync(
    baseQuery.ApplyQueryOptions(QueryOptions.WithSoftDeleted)
);
```

## Notes

- Entities with a `DeletedAt` value of `null` are considered active and included in queries by default.
- Entities with a non-`null` `DeletedAt` value are excluded from query results unless explicitly included.
- The soft delete pattern preserves data integrity and allows for entity recovery.
- Ensure to handle soft deletion in your application logic (e.g., by setting `DeletedAt` instead of removing records).