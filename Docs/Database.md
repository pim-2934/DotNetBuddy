# Database Layer

## Purpose

Encapsulates data access using Repository and Unit of Work patterns for separation of concerns and transactional safety.

We extend the power of LINQ with small helpers (extensions and repository utilities). We do not replace LINQ with a separate query language or a heavy specification layer.

## Interfaces

- `IEntity<TKey>`
  Basic entity contract that includes an `Id` of type `TKey`.

- `IRepository<T, TKey>`
  Generic data access for entities. Provides CRUD helpers and LINQ-friendly overloads that accept a query configuration lambda.

- `IUnitOfWork`
  Manages repositories and coordinates transactions.

## Setup

```csharp
builder.Services.AddBuddy();
builder.Services.AddBuddyEfExtension<DatabaseContext>();
```

## Usage

The `IUnitOfWork` interface provides access to your data through generic repositories. Use the repository to perform standard CRUD operations and to compose queries with standard LINQ (Where, Select, Include, OrderBy, etc.) via a query configuration lambda.

If you need to encapsulate custom queries or more advanced logic for specific entities, you can define a dedicated repository.

```csharp
public class SomeService(IUnitOfWork uow)
{
    public async Task DoSomethingAsync(CancellationToken ct)
    {
        var repo = uow.Repository<MyEntity, Guid>();

        // Compose with LINQ via the query configuration lambda
        var items = await repo.GetRangeAsync(
            q => q.Where(e => e.IsActive)
                  .OrderByDescending(e => e.CreatedAt),
            cancellationToken: ct
        );

        // perform actions
        await uow.SaveAsync(ct);
    }
}
```

## Querying with LINQ + Repository Helpers

- Compose queries using the configuration lambda (Func<IQueryable<T>, IQueryable<T>>).
- Use EF Core's `Include`/`ThenInclude` for navigation properties inside the lambda.
- Pass `QueryOptions` when needed (e.g., include soft-deleted rows) using the named argument `queryOptions:` or with the `ApplyQueryOptions` extension inside the lambda.

### Examples

Basic filtering and ordering:
```csharp
var repo = uow.Repository<MyEntity, Guid>();
var items = await repo.GetRangeAsync(
    q => q.Where(e => e.IsActive)
          .OrderByDescending(e => e.CreatedAt),
    cancellationToken: cancellationToken
);
```

Including related entities:
```csharp
using Microsoft.EntityFrameworkCore; // for Include/ThenInclude

var items = await repo.GetRangeAsync(
    q => q.Include(e => e.Category)
          .ThenInclude(c => c.Parent),
    cancellationToken: cancellationToken
);
```

Get by IDs with additional includes/filters:
```csharp
var ids = new [] { id1, id2, id3 };
var items = await repo.GetRangeAsync(
    ids,
    q => q.Include(e => e.RelatedDetails),
    cancellationToken: cancellationToken
);
```

Single item via query:
```csharp
var entity = await repo.GetAsync(
    q => q.Where(e => e.Code == code),
    cancellationToken: cancellationToken
);
```

Any/Count using a query:
```csharp
var hasActives = await repo.AnyAsync(
    q => q.Where(e => e.IsActive),
    cancellationToken: cancellationToken
);

var specialCount = await repo.CountAsync(
    q => q.Where(e => e.Type == MyType.Special),
    cancellationToken: cancellationToken
);
```

Paging with a query:
```csharp
int page = 2;
int pageSize = 10;

var paged = await repo.GetPagedAsync(
    q => q.Where(e => e.IsActive)
          .OrderBy(e => e.Name),
    page,
    pageSize,
    cancellationToken: cancellationToken
);
// paged.Items, paged.TotalCount, paged.PageNumber, paged.PageSize
```

Using query options (e.g., include soft-deleted rows):
```csharp
// Option A: pass QueryOptions via argument
var all = await repo.GetRangeAsync(queryOptions: QueryOptions.WithSoftDeleted, cancellationToken: cancellationToken);

// Option B: apply options inside the lambda
var all2 = await repo.GetRangeAsync(
    q => q.ApplyQueryOptions(QueryOptions.WithSoftDeleted),
    cancellationToken: cancellationToken
);
```

## Example (Custom Repository)

```csharp
public interface IFooRepository : IRepository<Foo, Guid>
{
    // Add methods here that return tasks or values.
    // Design guidance:
    // - Accept and/or build LINQ queries via the configuration lambda.
    // - Allow consumers to use Include/ThenInclude and other LINQ operators inside the lambda.
}
```

```csharp
public class FooRepository(DatabaseContext context) : Repository<Foo, Guid>(context), IFooRepository
{
    // Implement repository methods here that internally compose LINQ queries
    // and use the base methods like GetRangeAsync(q => ...), GetPagedAsync(q => ..., ...), etc.
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
- To include soft-deleted entities in queries, use the `QueryOptions.WithSoftDeleted` flag (either via method arguments or `ApplyQueryOptions`).
- Avoid static repositories or global helpers that might conflict with DbContext lifetime and transaction scope.

