using Microsoft.AspNetCore.Mvc;

namespace DotNetBuddy.Example.Tests.Entities;

public class ProblemDetailsResponseWithMetadata : ProblemDetails
{
    public Dictionary<string, object> Metadata { get; set; } = [];
}