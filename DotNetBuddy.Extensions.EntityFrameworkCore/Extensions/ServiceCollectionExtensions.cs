using System.Reflection;
using DotNetBuddy.Domain;
using DotNetBuddy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for adding custom services related to BuddyDotNet into an
/// <see cref="IServiceCollection"/> dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services related to BuddyDotNet to the dependency injection container.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the DbContext to be used by the BuddyDotNet services.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the services will be added.
    /// </param>
    /// <param name="assemblyNames">
    /// An array of additional <see cref="Assembly"/> instances used to locate and run service installers. Use this if
    /// the referenced project has installers but is not loaded yet at the time of adding buddy.
    /// </param>
    public static void AddBuddy<T>(this IServiceCollection services, params AssemblyName[] assemblyNames)
        where T : DbContext
    {
        services.AddBuddy(assemblyNames);
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork<T>>();
    }
}