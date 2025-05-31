using System.Linq.Expressions;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Example.Models;

namespace DotNetBuddy.Example.Repositories.Interfaces;

public interface IWeatherForecastRepository : IRepository<WeatherForecast, Guid>
{
    Task<IEnumerable<WeatherForecast>> GetLatestAsync(
        int top,
        QueryOptions options = QueryOptions.None,
        params Expression<Func<WeatherForecast, object>>[] includes
    );
}