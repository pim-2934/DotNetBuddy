using System.Linq.Expressions;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Infrastructure.Repositories;

/// <inheritdoc />
public class Repository<T, TKey>(DbContext context) : IRepository<T, TKey> where T : class, IEntity<TKey>
{
    /// <summary>
    /// Represents the set of entities of a specific type that can be queried or updated within the database context.
    /// Facilitates LINQ queries and database operations for managing entity data.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly DbSet<T> DbSet = context.Set<T>();

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    )
    {
        var query = DbSet.ApplyQueryIncludes(includes).ApplyQueryOptions(options);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync(
        Expression<Func<T?, bool>> predicate,
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
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryOptions options = QueryOptions.None)
    {
        return await DbSet.ApplyQueryOptions(options).AnyAsync(predicate);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(TKey id, QueryOptions options = QueryOptions.None)
    {
        return await DbSet.ApplyQueryOptions(options).AnyAsync(x => x.Id!.Equals(id));
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);

        return entity;
    }

    /// <inheritdoc />
    public async Task<List<T>> AddAsync(List<T> entities)
    {
        await DbSet.AddRangeAsync(entities);

        return entities;
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
    public async Task DeleteAsync(TKey id)
    {
        var entity = await GetAsync(id);
        if (entity != null)
        {
            DbSet.Remove(entity);
        }
    }
}