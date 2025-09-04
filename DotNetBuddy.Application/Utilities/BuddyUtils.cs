using System.Text;
using DotNetBuddy.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetBuddy.Application.Utilities;

/// <summary>
/// Provides utility methods to facilitate the execution of database seeders during application startup.
/// </summary>
public static class BuddyUtils
{
    /// <summary>
    /// Executes all registered seeders for the specified environment and initializes the database with the required data.
    /// </summary>
    /// <param name="provider">The application's service provider to resolve registered seeders.</param>
    /// <param name="env">The hosting environment information used to filter seeders by environment name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RunSeedersAsync(IServiceProvider provider, IHostEnvironment env,
        CancellationToken cancellationToken = default)
    {
        foreach (var seeder in provider.CreateScope().ServiceProvider.GetServices<ISeeder>())
        {
            if (!seeder.Environments.Contains(env.EnvironmentName))
            {
                continue;
            }

            await seeder.SeedAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Generates a deterministic GUID based on a string input using MD5 hashing.
    /// The resulting GUID is consistent for the same input string.
    /// </summary>
    /// <param name="identifier">The input string used to generate the deterministic GUID.</param>
    /// <returns>A GUID deterministically generated from the specified input string.</returns>
    public static Guid GenerateDeterministicGuid(string identifier)
    {
        var hash = System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(identifier));
        return new Guid(hash);
    }
}