using DotNetBuddy.Domain;
using DotNetBuddy.Example.Entities;

namespace DotNetBuddy.Example.Repositories.Interfaces;

public interface IWeatherForecastRepository : IRepository<WeatherForecast, Guid>;