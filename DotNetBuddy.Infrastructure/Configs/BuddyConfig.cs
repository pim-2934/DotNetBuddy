using DotNetBuddy.Domain;

namespace DotNetBuddy.Infrastructure.Configs;

/// <summary>
/// Represents the configuration settings for the BuddyDotNet application.
/// </summary>
/// <remarks>
/// This class is a concrete implementation of the <see cref="IConfig"/> interface
/// and is designed to encapsulate configuration properties specific to the BuddyDotNet system.
/// It can be extended to include additional configurations as needed.
/// </remarks>
public class BuddyConfig : IConfig
{
    /// <summary>
    /// Indicates whether database seeders should be executed automatically during the application startup process.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, the system will automatically run the defined seeders during boot-up.
    /// This can be useful for initializing the database with default data during development or testing environments.
    /// In production environments, this setting is typically <c>false</c> to prevent unintended data modifications.
    /// </remarks>
    public bool RunSeedersOnBoot { get; set; } = false;
}