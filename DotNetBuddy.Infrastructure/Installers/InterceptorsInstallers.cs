using DotNetBuddy.Application;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Infrastructure.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Infrastructure.Installers;

/// <summary>
/// Represents an installer responsible for configuring interceptors within the dependency injection container.
/// </summary>
/// <remarks>
/// This installer registers services related to database operation interceptors, such as auditing functionality.
/// </remarks>
[InstallPriority(3000)]
public class InterceptorsInstallers : IInstaller
{
    /// <summary>
    /// Configures services related to database operation interceptors.
    /// </summary>
    /// <param name="services">The collection of services to which the interceptor services will be added.</param>
    public void Install(IServiceCollection services)
    {
        services.AddScoped<AuditInterceptor>();
    }
}