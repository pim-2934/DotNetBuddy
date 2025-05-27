using System.Linq.Expressions;
using DotNetBuddy.Domain;

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
    /// <typeparam name="TEntity">The type of the entity implementing the IEntity interface.</typeparam>
    /// <typeparam name="TKey">The type of the unique key for the entity.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedOneAsync<TEntity, TKey>(
        IUnitOfWork unitOfWork,
        Expression<Func<TEntity, bool>> predicate,
        TEntity entity
    ) where TEntity : class, IEntity<TKey>
    {
        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(predicate))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entity);
        }
    }

    /// <summary>
    /// Seeds a single entity asynchronously if it does not already exist in the repository.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to manage database transactions and the repository for persisting the entity.</param>
    /// <param name="identifier">A unique string identifier used to generate a deterministic GUID for the entity.</param>
    /// <param name="entity">The entity to be seeded into the database if it does not already exist.</param>
    /// <returns>A task that represents the asynchronous operation for seeding the entity.</returns>
    public static async Task SeedOneAsync<TEntity, TKey>(IUnitOfWork unitOfWork, string identifier, TEntity entity)
        where TEntity : class, IEntity<TKey>
    {
        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(entity.Id))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entity);
        }
    }

    /// <summary>
    /// Adds a single entity to the repository if an entity with the specified identifier does not already exist.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing the repository and transaction.</param>
    /// <param name="id">The identifier used to check for the existence of the entity in the repository.</param>
    /// <param name="entity">The entity to add if no entity with the specified identifier exists.</param>
    /// <typeparam name="TEntity">The type of the entity, which must implement the <see cref="IEntity{TKey}"/> interface.</typeparam>
    /// <typeparam name="TKey">The type of the identifier for the entity.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedOneAsync<TEntity, TKey>(IUnitOfWork unitOfWork, TKey id, TEntity entity)
        where TEntity : class, IEntity<TKey>
    {
        entity.Id = id;

        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(id))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entity);
        }
    }

    /// <summary>
    /// Inserts multiple entities into the repository if no entities matching the specified predicate currently exist.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing the repository and transaction.</param>
    /// <param name="predicate">A function to test each entity for a condition to determine if any entities match existing records.</param>
    /// <param name="entities">The list of entities to insert if no matches are found.</param>
    /// <typeparam name="TEntity">The type of the entity implementing the <see cref="IEntity{TKey}"/> interface.</typeparam>
    /// <typeparam name="TKey">The type of the key associated with the entity.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedManyAsync<TEntity, TKey>(
        IUnitOfWork unitOfWork,
        Expression<Func<TEntity, bool>> predicate,
        List<TEntity> entities
    ) where TEntity : class, IEntity<TKey>
    {
        if (!await unitOfWork.Repository<TEntity, TKey>().AnyAsync(predicate))
        {
            await unitOfWork.Repository<TEntity, TKey>().AddAsync(entities);
        }
    }
}