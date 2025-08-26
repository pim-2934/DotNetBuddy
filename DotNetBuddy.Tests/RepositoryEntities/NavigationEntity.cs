using DotNetBuddy.Domain;

namespace DotNetBuddy.Tests.RepositoryEntities;

public class NavigationEntity : IEntity<Guid>
{
    public Guid Id { get; set; }

    public NavigationEntity? Parent { get; set; }

    public List<NavigationEntity> Children { get; set; } = [];
}