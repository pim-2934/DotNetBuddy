using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Example.Contracts;

public class LocationCreateDto
{
    [Required]
    public required string Name { get; init; }
}
