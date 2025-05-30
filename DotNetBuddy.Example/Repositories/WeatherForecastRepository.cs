using DotNetBuddy.Example.Models;
using DotNetBuddy.Example.Repositories.Interfaces;
using DotNetBuddy.Infrastructure.Repositories;

namespace DotNetBuddy.Example.Repositories;

public class WeatherForecastRepository(DatabaseContext context)
    : Repository<WeatherForecast, Guid>(context), IWeatherForecastRepository;