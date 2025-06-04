namespace DotNetBuddy.Tests.ExpressionPathEntities;

public class NestedEntity
{
    public int Id { get; set; }
    public required DeepNestedEntity DeepNested { get; set; }
    public List<DeepNestedEntity> DeepNestedItems { get; set; } = [];
}