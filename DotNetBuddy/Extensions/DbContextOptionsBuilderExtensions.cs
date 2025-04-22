using DotNetBuddy.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Extensions;

/// <summary>
/// Provides extension methods for configuring <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds Buddy interceptors, such as the <see cref="AuditInterceptor"/>, to the <see cref="DbContextOptionsBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="DbContextOptionsBuilder"/> to configure.</param>
    /// <param name="provider">The <see cref="IServiceProvider"/> used to resolve the services required for the interceptor.</param>
    /// <returns>The configured <see cref="DbContextOptionsBuilder"/> including the added interceptors.</returns>
    public static DbContextOptionsBuilder AddBuddyInterceptors(
        this DbContextOptionsBuilder builder,
        IServiceProvider provider
    )
    {
        var auditInterceptor = provider.GetRequiredService<AuditInterceptor>();
        builder.AddInterceptors(auditInterceptor);

        return builder;
    }
}