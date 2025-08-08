using DotNetBuddy.Domain;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Extensions.EntityFrameworkCore;

/// <inheritdoc />
public class UnitOfWork<TContext>(TContext context) : IUnitOfWork where TContext : DbContext
{
    private readonly Dictionary<Type, object> _repositories = new();

    /// <inheritdoc />
    public IRepository<T, TKey> Repository<T, TKey>() where T : class, IEntity<TKey>
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IRepository<T, TKey>) _repositories[typeof(T)];
        }

        var repositoryInstance = new Repository<T, TKey>(context);
        _repositories[typeof(T)] = repositoryInstance;

        return (IRepository<T, TKey>) _repositories[typeof(T)];
    }

    /// <inheritdoc />
    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        context.Dispose();
    }
}