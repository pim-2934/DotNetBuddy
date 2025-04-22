using System.ComponentModel.DataAnnotations;

namespace DotNetBuddy.Example.Configs;

public class WeatherForecastConfig : IConfig
{
    [Required]
    public required int OutputResults { get; init; } = 5;
}