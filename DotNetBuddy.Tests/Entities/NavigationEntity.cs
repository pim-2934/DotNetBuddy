using DotNetBuddy.Domain;

namespace DotNetBuddy.Tests.Entities;

public class NavigationEntity : IEntity<Guid>
{
    public Guid Id { get; set; }

    public NavigationEntity? Parent { get; set; }

    public List<NavigationEntity> Children { get; set; } = [];
}