using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Application;

/// <summary>
/// Represents a contract for service installers that configure and register
/// services into the dependency injection container.
/// </summary>
public interface IInstaller
{
    /// <summary>
    /// Configures services in the dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> instance to which services are added.
    /// </param>
    public void Install(IServiceCollection services);
}