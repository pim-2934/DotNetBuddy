using DotNetBuddy.Application.Utilities;
using DotNetBuddy.Domain.Attributes;

namespace DotNetBuddy.Tests.Unit;

public class SearchPredicateBuilderTests
{
    private class Root
    {
        [Searchable]
        public string? Name { get; set; }

        public Child? Child { get; set; }

        public List<Child> Children { get; set; } = [];
    }

    private class Child
    {
        [Searchable]
        public string? Title { get; set; }
    }

    [Fact]
    public void Build_WithNullOrWhitespace_ReturnsNull()
    {
        Assert.Null(SearchPredicateBuilder.Build<Root>(null!));
        Assert.Null(SearchPredicateBuilder.Build<Root>(""));
        Assert.Null(SearchPredicateBuilder.Build<Root>("   "));
    }

    [Fact]
    public void Build_Matches_DirectSearchableProperty()
    {
        var items = new[]
        {
            new Root { Name = "Alpha" },
            new Root { Name = "Beta" },
            new Root { Name = "Gamma" }
        };

        var expr = SearchPredicateBuilder.Build<Root>("Beta");
        Assert.NotNull(expr);

        var compiled = expr.Compile();
        var result = items.Where(compiled).ToList();

        Assert.Single(result);
        Assert.Equal("Beta", result[0].Name);
    }

    [Fact]
    public void Build_Matches_SingleNavigationProperty_WithNullSafeCheck()
    {
        var items = new[]
        {
            new Root { Name = "X", Child = null },
            new Root { Name = "Y", Child = new Child { Title = "Hello World" } },
            new Root { Name = "Z", Child = new Child { Title = "Other" } }
        };

        var expr = SearchPredicateBuilder.Build<Root>("World", true);
        Assert.NotNull(expr);

        var compiled = expr.Compile();
        var result = items.Where(compiled).ToList();

        Assert.Single(result);
        Assert.Equal("Y", result[0].Name);
    }

    [Fact]
    public void Build_Matches_CollectionNavigationProperty_WithNullSafeAny()
    {
        var items = new[]
        {
            new Root { Name = "X", Children = [] },
            new Root { Name = "Y", Children = [new Child { Title = "Foo Bar" }] },
            new Root { Name = "Z", Children = [new Child { Title = "Baz" }] }
        };

        var expr = SearchPredicateBuilder.Build<Root>("Bar", true);
        Assert.NotNull(expr);

        var compiled = expr.Compile();
        var result = items.Where(compiled).ToList();

        Assert.Single(result);
        Assert.Equal("Y", result[0].Name);
    }
}
