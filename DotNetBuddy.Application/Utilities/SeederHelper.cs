using System.Linq.Expressions;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Enums;

namespace DotNetBuddy.Application.Utilities;

/// <summary>
/// Provides helper methods for seeding entities into a repository with logic
/// to ensure that they do not already exist in the data store.
/// </summary>
public static class SeederHelper
{
    /// <summary>
    /// Inserts a single entity into the repository if no entity matching the specified predicate currently exists.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing the repository and transaction.</param>
    /// <param name="predicate">A function to test the entity for a condition to determine if a match exists in the repository.</param>
    /// <param name="entity">The entity to insert if no matches are found.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <typeparam name="TEntity">The type of the entity implementing the IEntity interface.</typeparam>
    /// <typeparam name="TKey">The type of the unique key for the entity.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedOneAsync<TEntity, TKey>(
        IUnitOfWork unitOfWork,
        Expression<Func<TEntity, bool>> predicate,
        TEntity entity,
        CancellationToken cancellationToken = default
    ) where TEntity : class, IEntity<TKey>
    {
        var query = unitOfWork.Repository<TEntity, TKey>().MakeQuery(QueryOptions.WithSoftDeleted).Where(predicate);

        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(query, cancellationToken))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// Seeds a single entity into the repository asynchronously if it does not already exist based on the specified conditions.
    /// </summary>
    /// <param name="unitOfWork">The unit of work that provides access to the repository and manages database transactions.</param>
    /// <param name="entity">The entity to be added to the repository if it does not already exist.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation if necessary.</param>
    /// <typeparam name="TEntity">The type of the entity to be seeded, implementing the IEntity interface.</typeparam>
    /// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
    /// <returns>A task representing the asynchronous operation of seeding the entity.</returns>
    public static async Task SeedOneAsync<TEntity, TKey>(IUnitOfWork unitOfWork, TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        var query = unitOfWork.Repository<TEntity, TKey>().MakeQuery(QueryOptions.WithSoftDeleted)
            .Where(x => x.Id!.Equals(entity.Id));

        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(query, cancellationToken))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// Adds a single entity to the repository if an entity with the specified identifier does not already exist.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing the repository and transaction.</param>
    /// <param name="id">The unique identifier used to check for the existence of the entity in the repository.</param>
    /// <param name="entity">The entity to add if no entity with the specified identifier exists.</param>
    /// <param name="cancellationToken">A token that allows the operation to be canceled.</param>
    /// <typeparam name="TEntity">The type of the entity, which must implement the <see cref="IEntity{TKey}"/> interface.</typeparam>
    /// <typeparam name="TKey">The type of the identifier for the entity.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedOneAsync<TEntity, TKey>(IUnitOfWork unitOfWork, TKey id, TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TKey>
    {
        entity.Id = id;

        var query = unitOfWork.Repository<TEntity, TKey>().MakeQuery(QueryOptions.WithSoftDeleted);

        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(query, cancellationToken))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// Inserts multiple entities into the repository if no entities matching the specified predicate currently exist.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing the repository and transaction.</param>
    /// <param name="predicate">A function to test each entity for a condition to determine if any entities match existing records.</param>
    /// <param name="entities">The list of entities to insert if no matches are found.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <typeparam name="TEntity">The type of the entity implementing the IEntity interface.</typeparam>
    /// <typeparam name="TKey">The type of the unique key for the entity.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedManyAsync<TEntity, TKey>(
        IUnitOfWork unitOfWork,
        Expression<Func<TEntity, bool>> predicate,
        List<TEntity> entities,
        CancellationToken cancellationToken = default
    ) where TEntity : class, IEntity<TKey>
    {
        var query = unitOfWork.Repository<TEntity, TKey>().MakeQuery(QueryOptions.WithSoftDeleted).Where(predicate);

        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(query, cancellationToken))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entities, cancellationToken);
        }
    }
}