using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;

namespace DotNetBuddy.Example.Entities;

public class Location : IAuditableEntity<Guid>, ISoftDeletableEntity<Guid>
{
    public Guid Id { get; set; }

    [Required, StringLength(255)] public required string Name { get; set; }

    public virtual ICollection<WeatherForecast> WeatherForecasts { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}