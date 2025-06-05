using System.Linq.Expressions;
using DotNetBuddy.Infrastructure.Utilities;
using DotNetBuddy.Tests.ExpressionPathEntities;

namespace DotNetBuddy.Tests.Unit;

public class ExpressionPathVisitorTests
{
    [Fact]
    public void GetPropertyPath_WithSimpleProperty_ReturnsPropertyName()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression = x => x.Name;

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Name", path);
    }

    [Fact]
    public void GetPropertyPath_WithNestedProperty_ReturnsNestedPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression = x => x.Parent!.Name;

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Parent.Name", path);
    }

    [Fact]
    public void GetPropertyPath_WithDeeplyNestedProperty_ReturnsFullPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.Nested!.DeepNested.Value;

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Nested.DeepNested.Value", path);
    }

    [Fact]
    public void GetPropertyPath_WithCollectionProperty_ReturnsCollectionPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression = x => x.Children;

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Children", path);
    }

    [Fact]
    public void GetPropertyPath_WithSelectOnCollection_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.RelatedItems.Select(r => r.Name);

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("RelatedItems.Name", path);
    }

    [Fact]
    public void GetPropertyPath_WithNestedSelectOnCollection_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.RelatedItems.Select(r => r.GrandChildren);

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("RelatedItems.GrandChildren", path);
    }

    [Fact]
    public void GetPropertyPath_WithMultiLevelSelect_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.RelatedItems.Select(r => r.GrandChildren.Select(g => g.Description));

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("RelatedItems.GrandChildren.Description", path);
    }

    [Fact]
    public void GetPropertyPath_WithComplexNestedPath_ReturnsCorrectFullPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.Nested!.DeepNestedItems.Select(d => d.FinalItems.Select(f => f.FinalValue));

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Nested.DeepNestedItems.FinalItems.FinalValue", path);
    }

    [Fact]
    public void GetPropertyPath_WithExplicitCast_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, string>> expression =
            x => x.Parent!.Name;

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Parent.Name", path);
    }

    [Fact]
    public void GetPropertyPath_WithImplicitCast_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.Id; // int will be implicitly cast to object

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Id", path);
    }

    [Fact]
    public void GetPropertyPath_WithNullChecking_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.Parent!.Name;

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Parent.Name", path);
    }

    [Fact]
    public void GetPropertyPath_WithMultipleNullChecking_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.Nested!.DeepNested.Value;

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("Nested.DeepNested.Value", path);
    }

    [Fact]
    public void GetPropertyPath_WithSameParameterNameInSelect_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.RelatedItems.Select(y => y.GrandChildren);

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("RelatedItems.GrandChildren", path);
    }

    [Fact]
    public void GetPropertyPath_WithMultipleSelectsAndSameParameterNames_ReturnsCorrectPath()
    {
        // Arrange
        Expression<Func<TestEntity, object>> expression =
            x => x.RelatedItems.Select(y => y.GrandChildren.Select(z => z.Description));

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("RelatedItems.GrandChildren.Description", path);
    }

    [Fact]
    public void GetPropertyPath_WithNestedSelectsWithDifferentParameterNames_ReturnsCorrectPath()
    {
        // Arrange - This is closest to your specific use case
        Expression<Func<TestEntity, object>> expression =
            x => x.RelatedItems.Select(a => a.GrandChildren.Select(b => b.Description));

        // Act
        var path = ExpressionPathVisitor.GetPropertyPath(expression);

        // Assert
        Assert.Equal("RelatedItems.GrandChildren.Description", path);
    }
}