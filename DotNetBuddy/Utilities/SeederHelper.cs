using System.Linq.Expressions;
using System.Text;

namespace DotNetBuddy.Utilities;

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
    /// <typeparam name="TEntity">The type of the entity implementing the <see cref="IEntity"/> interface.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedOneAsync<TEntity>(
        IUnitOfWork unitOfWork,
        Expression<Func<TEntity, bool>> predicate,
        TEntity entity
    ) where TEntity : class, IEntity
    {
        if (!await unitOfWork.Repository<TEntity>().AnyAsync(predicate))
        {
            await unitOfWork.Repository<TEntity>().AddAsync(entity);
        }
    }

    /// <summary>
    /// Seeds a single entity asynchronously if it does not already exist in the repository.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to manage database transactions and the repository for persisting the entity.</param>
    /// <param name="identifier">A unique string identifier used to generate a deterministic GUID for the entity.</param>
    /// <param name="entity">The entity to be seeded into the database if it does not already exist.</param>
    /// <returns>A task that represents the asynchronous operation for seeding the entity.</returns>
    public static async Task SeedOneAsync<TEntity>(IUnitOfWork unitOfWork, string identifier, TEntity entity)
        where TEntity : class, IEntity
    {
        var guid = GenerateDeterministicGuid(identifier);
        entity.Id = guid;

        if (!await unitOfWork.Repository<TEntity>().AnyAsync(guid))
        {
            await unitOfWork.Repository<TEntity>().AddAsync(entity);
        }
    }

    /// <summary>
    /// Seeds a single entity into the repository if an entity with the specified Guid does not already exist.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing the repository and transaction.</param>
    /// <param name="guid">The Guid to check for an existing entity in the repository.</param>
    /// <param name="entity">The entity to insert if no entity with the specified Guid exists.</param>
    /// <typeparam name="TEntity">The type of the entity implementing the <see cref="IEntity"/> interface.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedOneAsync<TEntity>(IUnitOfWork unitOfWork, Guid guid, TEntity entity)
        where TEntity : class, IEntity
    {
        entity.Id = guid;

        if (!await unitOfWork.Repository<TEntity>().AnyAsync(guid))
        {
            await unitOfWork.Repository<TEntity>().AddAsync(entity);
        }
    }

    /// <summary>
    /// Inserts a list of entities into the repository if no entities matching the specified predicate currently exist.
    /// </summary>
    /// <param name="unitOfWork">The unit of work managing the repository and transaction.</param>
    /// <param name="predicate">A function to test each entity for a condition to determine if any entities match existing records.</param>
    /// <param name="entities">The list of entities to insert if no matches are found.</param>
    /// <typeparam name="TEntity">The type of the entity implementing the <see cref="IEntity"/> interface.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SeedManyAsync<TEntity>(
        IUnitOfWork unitOfWork,
        Expression<Func<TEntity, bool>> predicate,
        List<TEntity> entities
    ) where TEntity : class, IEntity
    {
        if (!await unitOfWork.Repository<TEntity>().AnyAsync(predicate))
        {
            await unitOfWork.Repository<TEntity>().AddAsync(entities);
        }
    }

    /// <summary>
    /// Generates a deterministic GUID based on a string input using MD5 hashing.
    /// The same input string will always produce the same GUID.
    /// </summary>
    /// <param name="identifier">The string input to generate the deterministic GUID from.</param>
    /// <returns>A GUID that is deterministically generated from the input string.</returns>
    public static Guid GenerateDeterministicGuid(string identifier)
    {
        var hash = System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(identifier));
        return new Guid(hash);
    }
}