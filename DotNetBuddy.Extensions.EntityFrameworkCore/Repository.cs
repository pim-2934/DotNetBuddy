using System.Linq.Expressions;
using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Domain.Common;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Extensions.EntityFrameworkCore;

/// <inheritdoc />
public class Repository<T, TKey>(DbContext context) : IRepository<T, TKey> where T : class, IEntity<TKey>
{
    /// <summary>
    /// Represents the set of entities of a specific type that can be queried or updated within the database context.
    /// Facilitates LINQ queries and database operations for managing entity data.
    /// </summary>
    protected readonly DbSet<T> DbSet = context.Set<T>();

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetRangeAsync(
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes)
    {
        return await DbSet
            .ApplyQueryIncludes(includes)
            .ApplyQueryOptions(options)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetRangeAsync(QuerySpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetRangeAsync(
        Expression<Func<T, bool>> predicate,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    )
    {
        return await DbSet
            .ApplyQueryIncludes(includes)
            .ApplyQueryOptions(options)
            .Where(predicate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetRangeAsync(IEnumerable<TKey> ids, QuerySpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(x => ids.Contains(x.Id!)).ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetRangeAsync(
        IEnumerable<TKey> ids,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes)
    {
        return await DbSet
            .ApplyQueryIncludes(includes)
            .ApplyQueryOptions(options)
            .Where(x => ids.Contains(x.Id!))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<T, TKey>> GetPagedAsync(QuerySpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        if (spec.Page is null)
            throw new BuddyException("Page number must be set in the specification.");

        var totalCount = await DbSet.ApplySpecification(spec, false).CountAsync(cancellationToken);
        var items = await DbSet.ApplySpecification(spec).ToListAsync(cancellationToken);

        return new EntityPagedResult<T, TKey>(items, totalCount, spec.Page!.Value, spec.PageSize);
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<T, TKey>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes)
    {
        var query = DbSet.ApplyQueryIncludes(includes).ApplyQueryOptions(options);

        if (predicate is not null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new EntityPagedResult<T, TKey>(items, totalCount, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<T, TKey>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        IEnumerable<TKey> ids,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes)
    {
        var idsArray = ids.ToArray();
        var query = DbSet
            .ApplyQueryIncludes(includes)
            .ApplyQueryOptions(options)
            .Where(x => idsArray.Contains(x.Id!));

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new EntityPagedResult<T, TKey>(items, totalCount, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> SearchAsync(string searchTerm, QuerySpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        var predicate = BuildSearchPredicate(searchTerm);
        if (predicate is not null)
            spec.AddPredicate(predicate);

        return await GetRangeAsync(spec, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> SearchAsync(
        string searchTerm,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes)
    {
        var predicate = BuildSearchPredicate(searchTerm);
        if (predicate is null)
            return [];

        return await GetRangeAsync(predicate, options, includes);
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<T, TKey>> SearchPagedAsync(string searchTerm, QuerySpecification<T> spec,
        CancellationToken cancellationToken = default)
    {
        if (spec.Page is null)
            throw new BuddyException("Page number must be set in the specification.");

        var predicate = BuildSearchPredicate(searchTerm);
        if (predicate is not null)
            spec.AddPredicate(predicate);

        return await GetPagedAsync(spec, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<T, TKey>> SearchPagedAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes)
    {
        var predicate = BuildSearchPredicate(searchTerm);
        if (predicate is null)
            return new EntityPagedResult<T, TKey>([], 0, pageNumber, pageSize);

        return await GetPagedAsync(pageNumber, pageSize, predicate, options, includes);
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync(
        Expression<Func<T, bool>> predicate,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    )
    {
        return await DbSet
            .ApplyQueryIncludes(includes)
            .ApplyQueryOptions(options)
            .Where(predicate)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync(TKey id, QuerySpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplySpecification(spec).FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync(
        TKey id,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    )
    {
        return await DbSet
            .ApplyQueryIncludes(includes)
            .ApplyQueryOptions(options)
            .FirstOrDefaultAsync(x => x.Id!.Equals(id));
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplySpecification(spec).AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryOptions options = QueryOptions.None)
    {
        return await DbSet.ApplyQueryOptions(options).AnyAsync(predicate);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(TKey id, QuerySpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplySpecification(spec).AnyAsync(x => x.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(TKey id, QueryOptions options = QueryOptions.None)
    {
        return await DbSet.ApplyQueryOptions(options).AnyAsync(x => x.Id!.Equals(id));
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToArray();
        await DbSet.AddRangeAsync(entitiesList, cancellationToken);

        return entitiesList;
    }

    /// <inheritdoc />
    public void UpdateShallow(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
    }

    /// <inheritdoc />
    public void UpdateDeep(T entity)
    {
        DbSet.Update(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TKey id, bool forceHardDelete = false, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, MakeSpecification(), cancellationToken);

        if (entity != null)
        {
            if (!forceHardDelete && entity is ISoftDeletableEntity<TKey> softDeletable)
            {
                softDeletable.DeletedAt = DateTime.UtcNow;
                UpdateShallow(entity);
            }
            else
            {
                DbSet.Remove(entity);
            }
        }
    }

    /// <inheritdoc />
    public async Task DeleteRangeAsync(IEnumerable<TKey> ids, bool forceHardDelete = false,
        CancellationToken cancellationToken = default)
    {
        var entities = await GetRangeAsync(ids, MakeSpecification(), cancellationToken);

        var entitiesList = entities.ToArray();

        if (entitiesList.Length == 0)
            return;

        var softDeletableEntities =
            !forceHardDelete ? entitiesList.Where(e => e is ISoftDeletableEntity<TKey>).ToArray() : [];

        var hardDeletableEntities = entitiesList.Except(softDeletableEntities).ToArray();

        var deletedAt = DateTime.UtcNow;
        foreach (var entity in softDeletableEntities)
        {
            ((ISoftDeletableEntity<TKey>)entity).DeletedAt = deletedAt;
            UpdateShallow(entity);
        }

        if (hardDeletableEntities.Length > 0)
        {
            DbSet.RemoveRange(hardDeletableEntities);
        }
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(QuerySpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplySpecification(spec).CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        QueryOptions options = QueryOptions.None)
    {
        var query = DbSet.ApplyQueryOptions(options);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync();
    }

    /// <inheritdoc />
    public QuerySpecification<T> MakeSpecification()
    {
        return new QuerySpecification<T>();
    }

    /// <summary>
    /// Constructs a search predicate based on the provided search term. The predicate
    /// is used to perform filtering on searchable string properties, single navigation
    /// properties, and collection navigation properties of the given entity type.
    /// </summary>
    /// <param name="searchTerm">The search term used to build the filtering criteria.</param>
    /// <returns>An expression representing the search predicate, or null if the search term is null or empty.</returns>
    protected static Expression<Func<T, bool>>? BuildSearchPredicate(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return null;

        var entityParameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        // Get searchable properties directly on the entity
        var searchableProperties = typeof(T)
            .GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(SearchableAttribute), true).Length != 0)
            .ToList();

        // Process direct searchable string properties
        foreach (var property in searchableProperties)
        {
            if (property.PropertyType == typeof(string))
            {
                var propertyExpression = Expression.Property(entityParameter, property);
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                var searchTermExpression = Expression.Constant(searchTerm);
                var containsExpression = Expression.Call(propertyExpression, containsMethod!, searchTermExpression);

                combinedExpression = combinedExpression == null
                    ? containsExpression
                    : Expression.OrElse(combinedExpression, containsExpression);
            }
        }

        // Get single navigation properties (non-collection references)
        var singleNavigationProperties = typeof(T)
            .GetProperties()
            .Where(p =>
                p.PropertyType.IsClass &&
                p.PropertyType != typeof(string) &&
                !p.PropertyType.IsArray &&
                !IsCollectionType(p.PropertyType)
            )
            .ToList();

        // Process each single navigation property
        foreach (var navProperty in singleNavigationProperties)
        {
            var relatedType = navProperty.PropertyType;
            var relatedSearchableProperties = relatedType
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(SearchableAttribute), true).Length != 0 &&
                            p.PropertyType == typeof(string))
                .ToList();

            if (relatedSearchableProperties.Count == 0)
                continue;

            foreach (var relatedProperty in relatedSearchableProperties)
            {
                // Build a nested property access: x => x.RelatedEntity.SearchableProperty
                var relatedEntityExpression = Expression.Property(entityParameter, navProperty);

                // Check for null related entity to avoid null reference exceptions
                var nullCheck = Expression.NotEqual(relatedEntityExpression,
                    Expression.Constant(null, navProperty.PropertyType));
                var relatedPropertyExpression = Expression.Property(relatedEntityExpression, relatedProperty);

                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                var searchTermExpression = Expression.Constant(searchTerm);

                // Create condition: x.RelatedEntity != null && x.RelatedEntity.SearchableProperty.Contains(searchTerm)
                var containsExpression =
                    Expression.Call(relatedPropertyExpression, containsMethod!, searchTermExpression);
                var safeContainsExpression = Expression.AndAlso(nullCheck, containsExpression);

                combinedExpression = combinedExpression == null
                    ? safeContainsExpression
                    : Expression.OrElse(combinedExpression, safeContainsExpression);
            }
        }

        // Get collection navigation properties
        var collectionNavigationProperties = typeof(T)
            .GetProperties()
            .Where(p => IsCollectionType(p.PropertyType))
            .ToList();

        // Process each collection navigation property
        foreach (var navProperty in collectionNavigationProperties)
        {
            var collectionType = navProperty.PropertyType;
            Type elementType;

            // Get the element type from the collection
            if (collectionType.IsGenericType)
            {
                elementType = collectionType.GetGenericArguments()[0];
            }
            else if (collectionType.IsArray)
            {
                elementType = collectionType.GetElementType()!;
            }
            else
            {
                // Skip if we can't determine the element type
                continue;
            }

            var relatedSearchableProperties = elementType
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(SearchableAttribute), true).Length != 0 &&
                            p.PropertyType == typeof(string))
                .ToList();

            if (relatedSearchableProperties.Count == 0)
                continue;

            foreach (var relatedProperty in relatedSearchableProperties)
            {
                // Build an expression to check if any item in the collection has the search term
                // x => x.CollectionProperty != null && x.CollectionProperty.Any(item => item.SearchableProperty.Contains(searchTerm))

                var collectionExpression = Expression.Property(entityParameter, navProperty);

                // Check for null collection
                var nullCheck = Expression.NotEqual(collectionExpression,
                    Expression.Constant(null, navProperty.PropertyType));

                // Create the Any() method parameter: item => item.SearchableProperty.Contains(searchTerm)
                var itemParam = Expression.Parameter(elementType, "item");
                var itemProperty = Expression.Property(itemParam, relatedProperty);
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                var searchTermExpression = Expression.Constant(searchTerm);
                var containsExpression = Expression.Call(itemProperty, containsMethod!, searchTermExpression);
                var predicate = Expression.Lambda(containsExpression, itemParam);

                // Get the Any() method for the collection type
                var anyMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(elementType);

                // Create the Any() method call
                var anyExpression = Expression.Call(anyMethod, collectionExpression, predicate);

                // Combine with null check
                var safeAnyExpression = Expression.AndAlso(nullCheck, anyExpression);

                combinedExpression = combinedExpression == null
                    ? safeAnyExpression
                    : Expression.OrElse(combinedExpression, safeAnyExpression);
            }
        }

        return combinedExpression == null
            ? null
            : Expression.Lambda<Func<T, bool>>(combinedExpression, entityParameter);
    }

    private static bool IsCollectionType(Type type)
    {
        if (type == typeof(string))
            return false;

        // Check if the type is an array
        if (type.IsArray)
            return true;

        // Check if the type implements IEnumerable<T>
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(IEnumerable<>) ||
                genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IList<>))
            {
                return true;
            }
        }

        // Check interfaces to find IEnumerable<T> implementations
        return type.GetInterfaces().Any(i =>
            i.IsGenericType &&
            (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
             i.GetGenericTypeDefinition() == typeof(ICollection<>) ||
             i.GetGenericTypeDefinition() == typeof(IList<>)));
    }
}