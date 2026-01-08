using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Extensions.EntityFrameworkCore;

/// <inheritdoc />
public class UnitOfWork<TContext>(TContext context)
    : IUnitOfWork where TContext : DbContext
{
    private readonly Dictionary<Type, object> _repositories = new();

    /// <inheritdoc />
    public IRepository<T, TKey> Repository<T, TKey>() where T : class, IEntity<TKey>
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IRepository<T, TKey>)_repositories[typeof(T)];
        }

        var repositoryInstance = new Repository<T, TKey>(context);
        _repositories[typeof(T)] = repositoryInstance;

        return (IRepository<T, TKey>)_repositories[typeof(T)];
    }

    /// <inheritdoc />
    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var entity in entries)
        {
            var validationContext = new ValidationContext(entity);
            Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
        }

        return await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        context.Dispose();
    }
}