namespace DotNetBuddy.Domain.Attributes;

/// <summary>
/// Marks a property as searchable, making it included in automatic search operations.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SearchableAttribute : Attribute;