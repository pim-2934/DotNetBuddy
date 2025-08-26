# Database Layer

## Purpose

Encapsulates data access using Repository and Unit of Work patterns for separation of concerns and transactional safety.

We extend the power of LINQ with small helpers (extensions and repository utilities). We do not replace LINQ with a separate query language or a heavy specification layer.

## Interfaces

- `IEntity<TKey>`
  Basic entity contract that includes an `Id` of type `TKey`.

- `IRepository<T, TKey>`
  Generic data access for entities. Provides CRUD helpers and LINQ-friendly overloads that accept `IQueryable<T>`.

- `IUnitOfWork`
  Manages repositories and coordinates transactions.

## Setup

```csharp
builder.Services.AddBuddy();
builder.Services.AddBuddyEfExtension<DatabaseContext>();
```

## Usage

The `IUnitOfWork` interface provides access to your data through generic repositories. Use the repository to perform standard CRUD operations and to help start queries; compose the rest of the query with standard LINQ (Where, Select, Include, OrderBy, etc.).

If you need to encapsulate custom queries or more advanced logic for specific entities, you can define a dedicated repository.

```csharp
public class SomeService(IUnitOfWork uow)
{
    public async Task DoSomethingAsync(CancellationToken ct)
    {
        var repo = uow.Repository<MyEntity, Guid>();

        // Start with a base query (optionally with query options)
        var query = repo.MakeQuery(); // or: repo.MakeQuery(QueryOptions.WithSoftDeleted)

        // Compose with LINQ like normal
        query = query
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.CreatedAt);

        var items = await repo.GetRangeAsync(query, ct);
        // perform actions
        await uow.SaveAsync(ct);
    }
}
```

## Querying with LINQ + Repository Extensions

- Start queries with `MakeQuery(QueryOptions options = QueryOptions.None)` when you want a DbSet-based query with helper options applied.
- Compose the rest with LINQ. Use EF Core's `Include`/`ThenInclude` for navigation properties.
- Pass the resulting `IQueryable<T>` into repository methods like `GetRangeAsync`, `GetPagedAsync`, `GetAsync`, `AnyAsync`, and `CountAsync`.

### Examples

Basic filtering and ordering:
```csharp
var repo = uow.Repository<MyEntity, Guid>();
var query = repo.MakeQuery()
    .Where(e => e.IsActive)
    .OrderByDescending(e => e.CreatedAt);

var items = await repo.GetRangeAsync(query, cancellationToken);
```

Including related entities:
```csharp
using Microsoft.EntityFrameworkCore; // for Include/ThenInclude

var query = repo.MakeQuery()
    .Include(e => e.Category)
    .ThenInclude(c => c.Parent);

var items = await repo.GetRangeAsync(query, cancellationToken);
```

Get by IDs with additional includes/filters:
```csharp
var ids = new [] { id1, id2, id3 };
var query = repo.MakeQuery()
    .Include(e => e.RelatedDetails);

var items = await repo.GetRangeAsync(ids, query, cancellationToken);
```

Single item via query:
```csharp
var query = repo.MakeQuery().Where(e => e.Code == code);
var entity = await repo.GetAsync(query, cancellationToken);
```

Any/Count using a query:
```csharp
var hasActives = await repo.AnyAsync(repo.MakeQuery().Where(e => e.IsActive), cancellationToken);
var specialCount = await repo.CountAsync(repo.MakeQuery().Where(e => e.Type == MyType.Special), cancellationToken);
```

Paging with a query:
```csharp
int page = 2;
int pageSize = 10;

var query = repo.MakeQuery()
    .Where(e => e.IsActive)
    .OrderBy(e => e.Name);

var paged = await repo.GetPagedAsync(query, page, pageSize, cancellationToken);
// paged.Items, paged.TotalCount, paged.PageNumber, paged.PageSize
```

Using query options (e.g., include soft-deleted rows):
```csharp
var allQuery = repo.MakeQuery(QueryOptions.WithSoftDeleted);
var all = await repo.GetRangeAsync(allQuery, cancellationToken);
```

## Example (Custom Repository)

```csharp
public interface IFooRepository : IRepository<Foo, Guid>
{
    // Add methods here that return tasks or values.
    // Design guidance:
    // - Accept and/or build LINQ queries with IQueryable<Foo>.
    // - Use repo.MakeQuery(options) to start queries consistently.
    // - Let consumers use Include/ThenInclude and other LINQ operators.
}
```

```csharp
public class FooRepository(DatabaseContext context) : Repository<Foo, Guid>(context), IFooRepository
{
    // Implement repository methods here that internally compose LINQ queries
    // and use the base methods like GetRangeAsync(query), GetPagedAsync(query, ...), etc.
}
```

## Example (Extended Unit of Work)

When you introduce custom repositories, it is recommended to create an `ExtendedUnitOfWork` class to centralize and expose these custom repository instances.

### Definition

```csharp
public class ExtendedUnitOfWork(DatabaseContext context) : UnitOfWork<DatabaseContext>(context), IExtendedUnitOfWork
{
    public IFooRepository Foos { get; } = new FooRepository(context);
    public IBarRepository Bars { get; } = new BarRepository(context);
}
```

Registration:
```csharp
builder.Services.AddScoped<IExtendedUnitOfWork, ExtendedUnitOfWork>();
```

### Example Usage
```csharp
public class OrderService(IExtendedUnitOfWork uow)
{
    public async Task DoWorkAsync(CancellationToken ct)
    {
        var bars = await uow.Bars.GetRangeAsync(ct);
        // Custom logic
        await uow.SaveAsync(ct);
    }
}
```

## Notes

- Models must implement `IEntity<TKey>`.
- We extend LINQ; we do not replace it. Prefer composing queries using standard LINQ operators and EF Core `Include/ThenInclude`.
- Use `IUnitOfWork` when no custom queries are needed—it simplifies testing and keeps code generic.
- Prefer `ExtendedUnitOfWork` when working with multiple custom repositories to ensure cleaner service constructors and better discoverability.
- To include soft-deleted entities in queries, use `repo.MakeQuery(QueryOptions.WithSoftDeleted)` or the `ApplyQueryOptions(QueryOptions.WithSoftDeleted)` extension on `IQueryable<T>`.
- Avoid static repositories or global helpers that might conflict with DbContext lifetime and transaction scope.

