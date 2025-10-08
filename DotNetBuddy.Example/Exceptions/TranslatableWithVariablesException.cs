using DotNetBuddy.Domain.Exceptions;

namespace DotNetBuddy.Example.Exceptions;

public class TranslatableWithVariablesException(string detail, Dictionary<string, object>? metadata = null) :
    ResponseException(detail, 400, metadata);