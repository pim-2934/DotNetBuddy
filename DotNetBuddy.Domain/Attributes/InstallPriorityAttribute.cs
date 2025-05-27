namespace DotNetBuddy.Domain.Attributes;

/// <summary>
/// Specifies the installation priority of a class within the dependency injection process.
/// </summary>
/// <remarks>
/// This attribute is intended to be applied to classes that implement the IInstaller interface,
/// enabling control over the order in which installers are executed.
/// Installers with higher weights will be executed after installers with lower weights.
/// If the weight is not provided, the default value will be set to <c>int.MaxValue</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class InstallPriorityAttribute(int weight) : Attribute
{
    /// <summary>
    /// Gets the weight value associated with the installation priority.
    /// </summary>
    /// <remarks>
    /// The weight determines the execution order of installers during the dependency
    /// injection process. Installers with lower weight values will be executed first,
    /// followed by installers with higher weight values.
    /// If no weight is specified, the default value is <c>int.MaxValue</c>.
    /// </remarks>
    public int Weight { get; } = weight;
}