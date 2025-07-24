namespace DotNetBuddy.Tests.ExpressionPathEntities;

public class DeepNestedEntity
{
    public int Id { get; set; }
    public required string Value { get; set; }
    public List<FinalEntity> FinalItems { get; set; } = [];
}