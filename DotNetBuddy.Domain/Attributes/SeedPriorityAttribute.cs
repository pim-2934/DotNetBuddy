namespace DotNetBuddy.Domain.Attributes;

/// <summary>
/// An attribute used to define the priority of a seeder during its resolution or execution.
/// Seeders with lower priority values will be executed or resolved earlier than those with higher values.
/// </summary>
/// <remarks>
/// This attribute is intended for marking classes that implement the <see cref="ISeeder"/> interface.
/// The priority defined by this attribute helps determine the order of execution for multiple seeders.
/// </remarks>
/// <param name="priority">
/// The numeric value representing the priority of the seeder. Higher values indicate higher priority.
/// If a seeder class does not have this attribute, it will default to a priority of <see cref="int.MinValue"/>.
/// </param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SeedPriorityAttribute(int priority) : Attribute
{
    /// <summary>
    /// Gets the priority weight assigned to a seed operation or handler.
    /// </summary>
    /// Priority is used to determine the order in which seeders are executed or resolved,
    /// with higher values indicating higher priority. If no priority is specified,
    /// the default value is <see cref="int.MinValue"/>.
    public int Priority { get; } = priority;
}