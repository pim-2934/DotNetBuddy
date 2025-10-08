# Search Functionality

## Purpose

Enables intelligent search across entity properties by marking searchable fields with the `[Searchable]` attribute.

## Setup

```csharp
builder.Services.AddBuddy();
```


## Usage

### 1. Mark Properties as Searchable

```csharp
public class Product
{
    [Searchable]
    public string Name { get; set; }
    
    [Searchable]
    public string Description { get; set; }
    
    // Not searchable
    public decimal Price { get; set; }  
}
```


### 2. Use Search Extension Method

```csharp
// Using with repository and LINQ
var searchTerm = "smartphone";
var results = await repository.GetRangeAsync(
    q => q.Search(searchTerm),
    cancellationToken: ct
);
```


## Relationship Searching

To include properties from related entities in search:

```csharp
public class Product
{
    [Searchable]
    public string Name { get; set; }
    
    // Mark the relationship as searchable
    [Searchable]  
    public Category Category { get; set; }
}

public class Category
{
    [Searchable]
    public string Name { get; set; }
}
```


### Collection Navigation Properties

```csharp
public class Order
{
    [Searchable]
    public string OrderNumber { get; set; }
    
    // Mark the collection as searchable
    [Searchable]  
    public List<OrderItem> Items { get; set; } = [];
}
```


### Disable Relationship Searching

```csharp
// Only search direct properties
var results = await repository.GetRangeAsync(
    q => q.Search(searchTerm, includeRelations: false),
    cancellationToken: ct
);
```


## Notes

- Searches are case-insensitive.
- Related entities' searchable properties are included only if the relationship property is marked with `[Searchable]`.
- Collections like `List<T>` are supported when marked as `[Searchable]`.
- Use `includeRelations: false` for performance optimization when only searching direct properties.
- Null-safe checks are built in for all navigation properties.