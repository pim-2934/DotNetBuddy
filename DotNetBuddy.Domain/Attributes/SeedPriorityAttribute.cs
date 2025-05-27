namespace DotNetBuddy.Attributes;

/// <summary>
/// An attribute to specify a priority for seeders when being resolved or executed.
/// Classes marked with this attribute will be processed relative to the weight specified,
/// where lower values indicate higher priority.
/// </summary>
/// <remarks>
/// This attribute is intended for use with classes implementing the <see cref="ISeeder"/> interface.
/// It can be applied to influence the order in which seeders are resolved or executed during runtime.
/// </remarks>
/// <param name="weight">
/// The priority weight of the seeder; lower values indicate higher priority.
/// The default priority for seeders without this attribute is <see cref="int.MaxValue"/>.
/// </param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SeedPriorityAttribute(int weight) : Attribute
{
    /// <summary>
    /// Gets the priority weight assigned to a seeder.
    /// </summary>
    /// <remarks>
    /// The weight determines the priority of a seeder where lower values have higher priority.
    /// Seeders without a specified weight default to <see cref="int.MaxValue"/>.
    /// </remarks>
    public int Weight { get; } = weight;
}