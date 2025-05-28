using DotNetBuddy.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Infrastructure.Extensions;

/// <summary>
/// Provides extension methods for configuring <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds Buddy interceptors, such as the <see cref="AuditInterceptor"/>, to the <see cref="DbContextOptionsBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="DbContextOptionsBuilder"/> to configure.</param>
    /// <returns>The configured <see cref="DbContextOptionsBuilder"/> including the added interceptors.</returns>
    public static DbContextOptionsBuilder AddBuddyInterceptors(this DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditInterceptor());

        return builder;
    }
}