using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;

namespace DotNetBuddy.Tests;

public class TestAuditableEntity : IAuditableEntity<Guid>
{
    public Guid Id { get; set; }
    
    [Required, StringLength(100), Searchable]
    public required string Name { get; set; }
        
    [Searchable, StringLength(255)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}