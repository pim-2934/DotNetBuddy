using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Example.Contracts;

public class WeatherForecastUpdateDto
{
    public DateOnly Date { get; init; }
    public int TemperatureC { get; init; }

    [StringLength(255)]
    public string? Summary { get; set; }
}