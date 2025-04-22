using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuddyDotNet.Utilities;

/// <summary>
/// Provides utility methods to facilitate the execution of database seeders during application startup.
/// </summary>
public static class BuddyUtils
{
    /// <summary>
    /// Executes all registered seeders for the specified environment and initializes the database with the required data.
    /// </summary>
    /// <param name="unitOfWork">An instance of <see cref="IUnitOfWork"/> to manage database transactions and repositories.</param>
    /// <param name="provider">The application's service provider to resolve registered seeders.</param>
    /// <param name="env">The hosting environment information used to filter seeders by environment name.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task RunSeeders(IUnitOfWork unitOfWork, IServiceProvider provider, IHostEnvironment env)
    {
        foreach (var seeder in provider.GetServices<ISeeder>())
        {
            if (!seeder.Environments.Contains(env.EnvironmentName))
            {
                continue;
            }

            await seeder.SeedAsync(unitOfWork);
        }
    }
}