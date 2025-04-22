namespace BuddyDotNet.Configs;

/// <summary>
/// Represents the configuration for connection strings in the application.
/// </summary>
/// <remarks>
/// This class provides a container for storing and accessing the default database connection string.
/// It is designed to be used as part of the application's configuration structure and implements the <see cref="IConfig"/> interface.
/// </remarks>
public class ConnectionStringsConfig : IConfig
{
    /// <summary>
    /// Gets or sets the default database connection string.
    /// </summary>
    /// <remarks>
    /// This property is used to store the connection string for the application's default database.
    /// It is typically populated from configuration settings and can be used throughout the application to establish a database connection.
    /// </remarks>
    public string? DefaultConnection { get; set; }
}