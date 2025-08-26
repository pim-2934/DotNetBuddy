using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Common;
using DotNetBuddy.Domain.Enums;
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
    public async Task<IReadOnlyList<TEntity>> GetRangeAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetRangeAsync(IQueryable<TEntity> queryable,
        CancellationToken cancellationToken = default)
    {
        return await queryable.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetRangeAsync(IEnumerable<TKey> ids,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(x => ids.Contains(x.Id!)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEntityPagedResult<TEntity, TKey>> GetPagedAsync(IQueryable<TEntity> queryable, int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new EntityPagedResult<TEntity, TKey>(items, totalCount, page, pageSize);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetAsync(IQueryable<TEntity> queryable, CancellationToken cancellationToken = default)
    {
        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(IQueryable<TEntity> queryable, CancellationToken cancellationToken = default)
    {
        return await queryable.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Id!.Equals(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities,
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
        var entity = await GetAsync(id, cancellationToken);

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
        var entities = await GetRangeAsync(ids, cancellationToken);

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
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(IQueryable<TEntity> queryable, CancellationToken cancellationToken = default)
    {
        return await queryable.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IQueryable<TEntity> MakeQuery(QueryOptions options = QueryOptions.None)
    {
        return DbSet.ApplyQueryOptions(options).AsQueryable();
    }
}