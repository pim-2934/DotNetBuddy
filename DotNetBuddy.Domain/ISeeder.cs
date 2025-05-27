namespace DotNetBuddy.Domain;

/// <summary>
/// Represents a contract for implementing seeders, which populate data into a database
/// using a unit of work to manage transactions and repository operations.
/// </summary>
public interface ISeeder
{
    /// <summary>
    /// Gets the name of the environments for which the data seeder is intended.
    /// </summary>
    /// <remarks>
    /// This property is used to specify the target environments for the seeder,
    /// such as "Development", "Staging", or "Production". The value of this property
    /// determines whether the seeder should execute in the current environment
    /// during the application's startup or initialization.
    /// </remarks>
    public string[] Environments { get; }

    /// <summary>
    /// Seeds the database asynchronously by inserting required entities using the provided unit of work.
    /// </summary>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    public Task SeedAsync();
}