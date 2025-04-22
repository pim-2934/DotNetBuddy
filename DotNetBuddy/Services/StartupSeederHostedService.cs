using BuddyDotNet.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuddyDotNet.Services;

/// <summary>
/// A hosted service that executes configured seeders during application startup,
/// based on the active environment and registered seeder services.
/// </summary>
public class StartupSeederHostedService(IServiceProvider provider, IWebHostEnvironment env) : IHostedService
{
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        await BuddyUtils.RunSeeders(unitOfWork, provider, env);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}