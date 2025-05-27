using System.Reflection;
using DotNetBuddy.Application;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the dependency injection container.
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
        foreach (var assemblyName in assemblyNames)
        {
            Assembly.Load(assemblyName);
        }

        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork<T>>();

        FindAndRunInstallers(services);
    }

    private static void FindAndRunInstallers(IServiceCollection services)
    {
        var installers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany
            (
                a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                }
            )
            .Where(t => typeof(IInstaller).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select
            (t =>
                {
                    var attr = t!.GetCustomAttribute<InstallPriorityAttribute>();

                    return new
                    {
                        Type = t,
                        Priority = attr?.Weight ?? int.MaxValue
                    };
                }
            )
            .OrderBy(x => x.Priority)
            .Select(x => (IInstaller)Activator.CreateInstance(x.Type!)!);

        foreach (var installer in installers)
        {
            installer.Install(services);
        }
    }
}