using BuddyDotNet;
using BuddyDotNet.Utilities;
using DotNetBuddy.Example.Configs;
using DotNetBuddy.Example.Models;
using Microsoft.Extensions.Options;

namespace DotNetBuddy.Example.Seeders;

public class WeatherForecastSeeder(IOptions<WeatherForecastConfig> weatherForecastConfig) : ISeeder
{
    public string[] Environments =>
    [
        "Development",
        "Local",
        "Staging",
        "Production"
    ];

    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public async Task SeedAsync(IUnitOfWork unitOfWork)
    {
        await SeederHelper.SeedManyAsync
        (
            unitOfWork,
            x => x.Id != Guid.Empty, // Only seed if no entries
            Enumerable.Range(1, weatherForecastConfig.Value.OutputResults)
                .Select
                (
                    index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    }
                )
                .ToList()
        );

        await unitOfWork.SaveAsync();
    }
}