using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Common;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Extensions.EntityFrameworkCore;

/// <inheritdoc />
public class Repository<TEntity, TKey>(DbContext context)
    : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Represents the set of entities of a specific type that can be queried or updated within the database context.
    /// Facilitates LINQ queries and database operations for managing entity data.
    /// </summary>
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetRangeAsync(
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyQueryOptions(queryOptions).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetRangeAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await configureQuery(DbSet).ApplyQueryOptions(queryOptions).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetRangeAsync(
        IEnumerable<TKey> ids,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyQueryOptions(queryOptions).Where(x => ids.Contains(x.Id!))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetRangeAsync(
        IEnumerable<TKey> ids,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await configureQuery(DbSet)
            .ApplyQueryOptions(queryOptions)
            .Where(x => ids.Contains(x.Id!))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<TEntity, TKey>> GetPagedAsync(
        int page,
        int pageSize,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(x => x, page, pageSize, queryOptions, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<TEntity, TKey>> GetPagedAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        int page,
        int pageSize,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await configureQuery(DbSet).ApplyQueryOptions(queryOptions).CountAsync(cancellationToken);
        var items = await configureQuery(DbSet).ApplyQueryOptions(queryOptions)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new EntityPagedResult<TEntity, TKey>(items, totalCount, page, pageSize);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await configureQuery(DbSet).ApplyQueryOptions(queryOptions).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetAsync(
        TKey id,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyQueryOptions(queryOptions)
            .FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetAsync(
        TKey id,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await configureQuery(DbSet)
            .ApplyQueryOptions(queryOptions)
            .FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await configureQuery(DbSet).ApplyQueryOptions(queryOptions).AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(
        TKey id,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyQueryOptions(queryOptions).AnyAsync(x => x.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> AddAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToArray();
        await DbSet.AddRangeAsync(entitiesList, cancellationToken);

        return entitiesList;
    }

    /// <inheritdoc />
    public void UpdateShallow(TEntity entity)
    {
        context.Entry(entity).State = EntityState.Modified;
    }

    /// <inheritdoc />
    public void UpdateDeep(TEntity entity)
    {
        DbSet.Update(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TKey id, bool forceHardDelete = false, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, QueryOptions.None, cancellationToken);

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
    public async Task DeleteRangeAsync(
        IEnumerable<TKey> ids,
        bool forceHardDelete = false,
        CancellationToken cancellationToken = default)
    {
        var entities = await GetRangeAsync(ids, QueryOptions.None, cancellationToken);

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
    public async Task<int> CountAsync(
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ApplyQueryOptions(queryOptions).CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery,
        QueryOptions queryOptions = QueryOptions.None,
        CancellationToken cancellationToken = default)
    {
        return await configureQuery(DbSet).ApplyQueryOptions(queryOptions).CountAsync(cancellationToken);
    }
}