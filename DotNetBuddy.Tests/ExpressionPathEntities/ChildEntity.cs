namespace DotNetBuddy.Tests.ExpressionPathEntities;

public class ChildEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<GrandChildEntity> GrandChildren { get; set; } = [];
}