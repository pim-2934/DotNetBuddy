﻿namespace DotNetBuddy.Domain.Attributes;

/// <summary>
/// Defines the priority level of a class during the installation sequence in dependency injection.
/// </summary>
/// <remarks>
/// This attribute is used to annotate classes that participate in the installation process,
/// allowing their execution order to be determined based on a specified priority value.
/// Classes with lower priority values will be executed earlier, and the default priority is set
/// to <c>Int32.MaxValue</c> if no specific value is provided.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class InstallPriorityAttribute(int priority) : Attribute
{
    /// <summary>
    /// Gets the priority level assigned to determine the execution order of an installer.
    /// </summary>
    /// <remarks>
    /// Installers with a lower priority value will execute earlier during the dependency injection
    /// process, while those with higher priority values will execute later. If no priority is explicitly
    /// assigned, the default value is <c>int.MaxValue</c>.
    /// </remarks>
    public int Priority { get; } = priority;
}