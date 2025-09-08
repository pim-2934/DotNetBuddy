using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotNetBuddy.Domain;

namespace DotNetBuddy.Example.Models;

public sealed class WeatherForecast : IAuditableEntity<Guid>, ISoftDeletableEntity<Guid>
{
    // IEntity
    public Guid Id { get; set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // ISoftDeletableEntity
    public DateTime? DeletedAt { get; set; }

    // Data
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }

    [StringLength(255)]
    public string? Summary { get; set; }

    public Location? Location { get; set; }

    [NotMapped]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}