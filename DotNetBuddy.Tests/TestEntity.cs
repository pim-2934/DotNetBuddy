using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;

namespace DotNetBuddy.Tests;

public class TestEntity : IEntity<Guid>
{
    public Guid Id { get; set; }
        
    [Required, StringLength(100), Searchable]
    public string Name { get; set; } = string.Empty;
        
    [Searchable]
    public string? Description { get; set; }
        
    public DateTime CreatedAt { get; set; }
}