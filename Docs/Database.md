# Database Layer

## Purpose

Encapsulates data access using Repository and Unit of Work patterns for separation of concerns and transactional safety.

## Interfaces

- `IEntity`  
  Basic entity with `Guid Id`.

- `IRepository<T>`  
  Generic data access for entities.

- `IUnitOfWork`  
  Manages repositories and coordinates transactions.

## Setup

```csharp
builder.Services.AddBuddy();
builder.Services.AddBuddyEfExtension<DatabaseContext>();
```

## Usage

The `IUnitOfWork` interface provides access to all your data through generic repositories. This is the default way to perform standard CRUD operations across your entities.

If you need to encapsulate custom queries or more advanced logic for specific entities, you can define a dedicated repository.

```csharp
public class SomeService(IUnitOfWork uow)
{
    public async Task DoSomethingAsync()
    {
        var items = await uow.Repository<MyEntity, Guid>().GetRangeAsync();
        // perform actions
        await _uow.SaveAsync();
    }
}
```

## Querying with QuerySpecification

QuerySpecification<T> centralizes filtering (Predicates), includes (Includes), ordering (OrderBy), options (Options), and paging (Page/PageSize). The repository exposes overloads that accept a specification and is the preferred approach. Older overloads with predicates/options/includes are marked [Obsolete].

- Basic filtering, including, and ordering:
```csharp
var repo = uow.Repository<MyEntity, Guid>();
var spec = repo.MakeSpecification()
    .AddPredicate(e => e.IsActive)
    .AddInclude(e => e.Category)
    .AddOrderBy(e => e.CreatedAt, SortDirection.Descending)
    .SetOptions(QueryOptions.AsNoTracking);

var items = await repo.GetRangeAsync(spec);
```

- Get by IDs with a specification:
```csharp
var ids = new [] { id1, id2, id3 };
var spec = repo.MakeSpecification().AddInclude(e => e.RelatedDetails);
var items = await repo.GetRangeAsync(ids, spec);
```

- Single item by spec:
```csharp
var spec = repo.MakeSpecification().AddPredicate(e => e.Code == code);
var entity = await repo.GetAsync(spec);
```

- Any/Count by spec:
```csharp
var hasActives = await repo.AnyAsync(repo.MakeSpecification().AddPredicate(e => e.IsActive));
var count = await repo.CountAsync(repo.MakeSpecification().AddPredicate(e => e.Type == MyType.Special));
```

- Paging with spec:
```csharp
var spec = repo.MakeSpecification()
    .AddPredicate(e => e.IsActive)
    .AddOrderBy(e => e.Name)
    .SetPage(pageNumber: 2, pageSize: 10);

var paged = await repo.GetPagedAsync(spec);
// paged.Items, paged.TotalCount, paged.PageNumber, paged.PageSize, etc.
```

- Searching with spec:
```csharp
// Properties marked with [Searchable] are used by the search builder.
var spec = repo.MakeSpecification()
    .AddInclude(e => e.Category);

var results = await repo.SearchAsync("foo", spec);

// Paged search
var paged = await repo.SearchPagedAsync("foo", spec.SetPage(1, 20));
```

Migration note: Prefer the specification-based methods. The legacy overloads that take predicate/options/includes remain for backward compatibility but are obsolete and may be removed in a future version.

## Example (Custom Repository)

```csharp
public interface IFooRepository : IRepository<Foo, Guid>
{
    // Add methods here.
    // 
    // Guidelines (Specification-based):
    // - Prefer accepting a QuerySpecification<Foo> for read/query methods.
    // - Internally use DbSet.ApplySpecification(spec) for filtering, includes, ordering, options, and paging.
    // - Avoid exposing raw include arrays or option flags in public APIs; keep them inside the specification.
    // 
    // This ensures that:
    // - Consumers can control which related entities are loaded in a type-safe way via spec.AddInclude(...).
    // - Consumers can apply sorting, paging, and filtering consistently via the specification.
    // - Query composition remains reusable, testable, and discoverable.
}
```

```csharp
public class FooRepository(DatabaseContext context) : Repository<Foo, Guid>(context), IFooRepository
{
    // Implement repository methods here following the specification-based guidelines above.
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
    public async Task DoWorkAsync()
    {
        var bars = await uow.Bars.GetRangeAsync();
        // Custom logic
        await _uow.SaveAsync();
    }
}
```

## Notes

- Models must implement `IEntity` directly or indirectly.
- Prefer specification-based repository methods (GetRangeAsync(spec), GetPagedAsync(spec), SearchAsync(searchTerm, spec), etc.). Legacy overloads with predicate/options/includes are [Obsolete].
- Use `AddInclude(e => e.Nav.Prop)` on QuerySpecification to include related data. Includes use type-safe expressions and are converted internally via ExpressionPathVisitor.
- Search methods rely on properties marked with `[Searchable]` and the SearchPredicateBuilder to construct search expressions.
- Use `IUnitOfWork` when no custom queries are needed—it simplifies testing and keeps code generic.
- Prefer `ExtendedUnitOfWork` when working with multiple custom repositories to ensure cleaner service constructors and better discoverability.
- Avoid using static repositories or data access helpers to prevent issues with DbContext lifetime and transaction scope.

