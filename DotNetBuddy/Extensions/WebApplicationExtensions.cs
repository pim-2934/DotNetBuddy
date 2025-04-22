using Microsoft.AspNetCore.Builder;

namespace BuddyDotNet.Extensions;

/// <summary>
/// Provides extension methods for the WebApplication class, enabling additional functionalities.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the application to use exception handlers.
    /// </summary>
    /// <param name="app">The WebApplication instance this method extends.</param>
    public static void UseBuddyExceptions(this WebApplication app)
    {
        app.UseExceptionHandler();
    }
}