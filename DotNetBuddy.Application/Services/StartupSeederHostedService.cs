using DotNetBuddy.Application.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DotNetBuddy.Application.Services;

/// <summary>
/// A hosted service that executes configured seeders during application startup,
/// based on the active environment and registered seeder services.
/// </summary>
public class StartupSeederHostedService(IServiceProvider provider, IWebHostEnvironment env) : IHostedService
{
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await BuddyUtils.RunSeeders(provider, env);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}