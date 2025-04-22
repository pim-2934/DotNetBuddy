using Microsoft.EntityFrameworkCore;

namespace BuddyDotNet.Repositories;

/// <inheritdoc />
public class UnitOfWork<TContext>(TContext context) : IUnitOfWork where TContext : DbContext
{
    private readonly Dictionary<Type, object> _repositories = new();

    /// <inheritdoc />
    public IRepository<T> Repository<T>() where T : class, IEntity
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IRepository<T>) _repositories[typeof(T)];
        }

        var repositoryInstance = new Repository<T>(context);
        _repositories[typeof(T)] = repositoryInstance;

        return (IRepository<T>) _repositories[typeof(T)];
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