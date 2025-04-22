using BuddyDotNet.Repositories;
using DotNetBuddy.Example.Models;
using DotNetBuddy.Example.Repositories.Interfaces;

namespace DotNetBuddy.Example.Repositories;

public class WeatherForecastRepository(DatabaseContext context)
    : Repository<WeatherForecast>(context), IWeatherForecastRepository;