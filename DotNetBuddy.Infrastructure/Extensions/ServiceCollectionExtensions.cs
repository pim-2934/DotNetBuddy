using System.Reflection;
using DotNetBuddy.Application;
using DotNetBuddy.Domain.Attributes;
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
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the services will be added.
    /// </param>
    /// <param name="assemblyNames">
    /// An array of additional <see cref="Assembly"/> instances used to locate and run service installers. Use this if
    /// the referenced project has installers but is not loaded yet at the time of adding buddy.
    /// </param>
    public static void AddBuddy(this IServiceCollection services, params AssemblyName[] assemblyNames)
    {
        foreach (var assemblyName in assemblyNames)
        {
            Assembly.Load(assemblyName);
        }
        
        FindAndRunInstallers(services);
    }

    private static void FindAndRunInstallers(IServiceCollection services)
    {
        var installerTypes = AppDomain.CurrentDomain.GetAssemblies()
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
                        Priority = attr?.Priority ?? int.MaxValue
                    };
                }
            )
            .OrderByDescending(x => x.Priority)
            .ToList();

        foreach (var installer in installerTypes)
        {
            services.AddTransient(installer.Type!);
        }

        
        foreach (var installer in installerTypes)
        {
            using var serviceProvider = services.BuildServiceProvider();
            var resolvedInstaller = (IInstaller)serviceProvider.GetRequiredService(installer.Type!);
            resolvedInstaller.Install(services);
        }
    }
}