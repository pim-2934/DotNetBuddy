using System.Reflection;
using DotNetBuddy.Application;
using DotNetBuddy.Application.Services;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Infrastructure.Configs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotNetBuddy.Infrastructure.Installers;

/// <summary>
/// Responsible for registering all implementations of <see cref="ISeeder"/>
/// and optionally configuring their execution during application startup.
/// </summary>
[InstallPriority(3000)]
public class SeedersInstaller : IInstaller
{
    /// <summary>
    /// Installs seeders into the service collection by dynamically discovering and registering
    /// all types implementing the <see cref="ISeeder"/> interface from the application domain's assemblies.
    /// Optionally adds a hosted service to execute seeders if configured to run on application startup.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add seeders and optionally the hosted service to.
    /// </param>
    public void Install(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();

        var buddyConfig = provider.GetRequiredService<IOptions<BuddyConfig>>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var seederTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where
            (t =>
                t is { IsClass: true, IsAbstract: false, IsPublic: true } &&
                typeof(ISeeder).IsAssignableFrom(t)
            )
            .Select(t => new
            {
                Type = t,
                Priority = t.GetCustomAttribute<SeedPriorityAttribute>()?.Weight ?? int.MaxValue
            })
            .OrderBy(x => x.Priority)
            .Select(x => x.Type)
            .ToList();

        foreach (var type in seederTypes)
        {
            services.AddScoped(typeof(ISeeder), type);
        }

        if (buddyConfig.Value.RunSeedersOnBoot)
        {
            services.AddHostedService<StartupSeederHostedService>();
        }
    }
}