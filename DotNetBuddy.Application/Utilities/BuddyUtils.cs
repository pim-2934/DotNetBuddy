using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetBuddy.Utilities;

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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RunSeeders(IServiceProvider provider, IHostEnvironment env)
    {
        foreach (var seeder in provider.CreateScope().ServiceProvider.GetServices<ISeeder>())
        {
            if (!seeder.Environments.Contains(env.EnvironmentName))
            {
                continue;
            }

            await seeder.SeedAsync();
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