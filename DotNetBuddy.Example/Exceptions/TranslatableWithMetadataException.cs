using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Domain.Exceptions;

namespace DotNetBuddy.Example.Exceptions;

public class TranslatableWithMetadataException(string detail) : ResponseException(detail, 400)
{
    [ResponseMetadata]
    public required string Foo { get; set; }
}