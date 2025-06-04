namespace DotNetBuddy.Tests.ExpressionPathEntities;

public class TestEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public TestEntity? Parent { get; set; }
    public List<TestEntity> Children { get; set; } = [];
    public List<ChildEntity> RelatedItems { get; set; } = [];
    public NestedEntity? Nested { get; set; }
}