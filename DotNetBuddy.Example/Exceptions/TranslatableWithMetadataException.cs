using DotNetBuddy.Application.Exceptions;
using DotNetBuddy.Domain.Attributes;

namespace DotNetBuddy.Example.Exceptions;

public class TranslatableWithMetadataException(string detail) : ResponseException(detail, 400)
{
    [ResponseMetadata]
    public required string Foo { get; set; }
}