using DotNetBuddy.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace DotNetBuddy.Presentation.Extensions;

/// <summary>
/// Provides extension methods for the WebApplication class, enabling additional functionalities.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the application to use exception handlers using BuddyExceptionMiddleware.
    /// </summary>
    /// <param name="app">The WebApplication instance this method extends.</param>
    public static void UseBuddyExceptions(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}