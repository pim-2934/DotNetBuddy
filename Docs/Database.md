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

## Example (Custom Repository)

```csharp
public interface IFooRepository : IRepository<Foo, Guid>
{
    // Add methods here.
    // 
    // Guidelines:
    // - Methods should accept query options (IQueryOptions) and include expressions (IEnumerable<Expression<Func<Foo, object>>>) as parameters when appropriate.
    // - Use DbSet.ApplyQueryIncludes(includes).ApplyQueryOptions(options) to apply includes and filtering, sorting, paging, etc.
    // 
    // This ensures that:
    // - Consumers can control which related entities are loaded (avoiding over-fetching).
    // - Consumers can apply sorting, paging, and filtering in a consistent way.
    // - Query composition remains reusable and testable.
}
```

```csharp
public class FooRepository(DatabaseContext context) : Repository<Foo, Guid>(context), IFooRepository
{
    // Implement repository methods here following the guidelines above.
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
- Use `IUnitOfWork` when no custom queries are needed—it simplifies testing and keeps code generic.
- Prefer `ExtendedUnitOfWork` when working with multiple custom repositories to ensure cleaner service constructors and better discoverability.
- Avoid using static repositories or data access helpers to prevent issues with DbContext lifetime and transaction scope.

