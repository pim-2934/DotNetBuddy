namespace DotNetBuddy.Domain.Attributes;

/// <summary>
/// An attribute that can be applied to properties to designate them as metadata elements
/// in a response model.
/// </summary>
/// <remarks>
/// This attribute is intended to be used for marking properties that serve as meta
/// information within a response object. The presence of the attribute signifies that
/// the decorated property holds metadata that may be relevant for consumers of the response.
/// </remarks>
/// <example>
/// Apply this attribute to the relevant properties in a response model to identify
/// and differentiate metadata properties.
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public class ResponseMetadataAttribute : Attribute;