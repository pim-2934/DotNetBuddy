using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;

namespace DotNetBuddy.Example.Configs;

public class WeatherForecastConfig : IConfig
{
    [Required]
    public required int OutputResults { get; init; } = 5;
}