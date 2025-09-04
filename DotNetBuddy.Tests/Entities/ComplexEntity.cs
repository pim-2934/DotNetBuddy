using DotNetBuddy.Domain;

namespace DotNetBuddy.Tests.Entities;

public class ComplexEntity : IEntity<Guid>
{
    public Guid Id { get; set; }
    public int BaseValue { get; set; }
    public int BelowBaseValue { get; set; }
    public int UnchangeableValue { get; set; }
}