using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DotNetBuddy.Domain;

namespace DotNetBuddy.Example.Models;

public class WeatherForecast : IAuditableEntity<Guid>
{
    // IEntity
    public Guid Id { get; set; }
    
    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Data
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }

    [NotMapped]
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

    [StringLength(255)]
    public string? Summary { get; set; }
}