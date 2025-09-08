using DotNetBuddy.Example.Entities;
using DotNetBuddy.Extensions.EntityFrameworkCore;
using DotNetBuddy.Example.Repositories.Interfaces;

namespace DotNetBuddy.Example.Repositories;

public class WeatherForecastRepository(DatabaseContext context)
    : Repository<WeatherForecast, Guid>(context), IWeatherForecastRepository;