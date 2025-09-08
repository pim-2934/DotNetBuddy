using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Example.Contracts;

public class WeatherForecastUpdateDto
{
    [Required]
    public DateOnly? Date { get; init; }

    [Required]
    public int TemperatureC { get; init; }

    [StringLength(255)]
    public string? Summary { get; set; }
}