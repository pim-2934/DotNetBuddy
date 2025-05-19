using System.Linq.Expressions;
using DotNetBuddy.Enums;
using DotNetBuddy.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Repositories;

/// <inheritdoc />
public class Repository<T, TKey>(DbContext context) : IRepository<T, TKey> where T : class, IEntity<TKey>
{
    /// <summary>
    /// Represents the set of entities of a specific type that can be queried or updated within the database context.
    /// Facilitates LINQ queries and database operations for managing entity data.
    /// </summary>
    private readonly DbSet<T> _dbSet = context.Set<T>();

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    )
    {
        var query = includes.Aggregate<Expression<Func<T, object>>, IQueryable<T>>
        (
            _dbSet,
            (current, include) => current.Include(include)
        );

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await ApplyQueryOptions(query, options).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync(
        Expression<Func<T?, bool>> predicate,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    )
    {
        var query = includes.Aggregate<Expression<Func<T, object>>, IQueryable<T>>
        (
            _dbSet,
            (current, include) => current.Include(include)
        );

        return await ApplyQueryOptions(query, options).Where(predicate).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync(
        TKey id,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<T, object>>[] includes
    )
    {
        var query = includes.Aggregate<Expression<Func<T, object>>, IQueryable<T>>
        (
            _dbSet,
            (current, include) => current.Include(include)
        );

        return await ApplyQueryOptions(query, options).FirstOrDefaultAsync(x => EqualityComparer<TKey>.Default.Equals(x.Id, id));
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryOptions options = QueryOptions.None)
    {
        return await ApplyQueryOptions(_dbSet, options).AnyAsync(predicate);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(TKey id, QueryOptions options = QueryOptions.None)
    {
        return await ApplyQueryOptions(_dbSet, options).AnyAsync(x => EqualityComparer<TKey>.Default.Equals(x.Id, id));
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);

        return entity;
    }

    /// <inheritdoc />
    public async Task<List<T>> AddAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);

        return entities;
    }

    /// <inheritdoc />
    public Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);

        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TKey id)
    {
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    /// <summary>
    /// Applies specific query options to an IQueryable instance, modifying its behavior
    /// based on the provided flags for tracking, query filters, and execution strategy.
    /// </summary>
    /// <param name="query">The query to which the options will be applied.</param>
    /// <param name="options">The options defining how the query should be modified.</param>
    /// <returns>The modified query with the specified options applied.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    protected IQueryable<T> ApplyQueryOptions(IQueryable<T> query, QueryOptions options)
    {
        if (options.HasFlag(QueryOptions.AsNoTracking) &&
            options.HasFlag(QueryOptions.AsNoTrackingWithIdentityResolution))
        {
            throw new BuddyException
            (
                "Invalid query options.",
                "Cannot specify both AsNoTracking and AsNoTrackingWithIdentityResolution."
            );
        }

        if (options.HasFlag(QueryOptions.AsNoTracking))
        {
            query = query.AsNoTracking();
        }

        if (options.HasFlag(QueryOptions.AsNoTrackingWithIdentityResolution))
        {
            query = query.AsNoTrackingWithIdentityResolution();
        }

        if (options.HasFlag(QueryOptions.IgnoreQueryFilters))
        {
            query = query.IgnoreQueryFilters();
        }

        if (options.HasFlag(QueryOptions.UseSplitQuery))
        {
            query = query.AsSplitQuery();
        }

        return query;
    }
}