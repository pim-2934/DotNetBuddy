using DotNetBuddy.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for configuring and registering services
/// within an <see cref="IServiceCollection"/> to support Entity Framework Core operations
/// and repository/unit of work patterns.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Buddy EF Core extension to the dependency injection container.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the DbContext.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the services will be added.
    /// </param>
    public static void AddBuddyEfExtension<T>(this IServiceCollection services)
        where T : DbContext
    {
        services.TryAddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.TryAddScoped<IUnitOfWork, UnitOfWork<T>>();
    }
}