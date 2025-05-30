﻿using System.Linq.Expressions;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Enums;
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
        var query = includes.Aggregate<Expression<Func<T, object>>, IQueryable<T>>
        (
            DbSet,
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
            DbSet,
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
            DbSet,
            (current, include) => current.Include(include)
        );

        return await ApplyQueryOptions(query, options).FirstOrDefaultAsync(x => x.Id!.Equals(id));
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, QueryOptions options = QueryOptions.None)
    {
        return await ApplyQueryOptions(DbSet, options).AnyAsync(predicate);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(TKey id, QueryOptions options = QueryOptions.None)
    {
        return await ApplyQueryOptions(DbSet, options).AnyAsync(x => x.Id!.Equals(id));
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