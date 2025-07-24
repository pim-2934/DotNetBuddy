using System.Linq.Expressions;
using DotNetBuddy.Extensions.EntityFrameworkCore;
using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Example.Models;
using DotNetBuddy.Example.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Example.Repositories;

public class WeatherForecastRepository(DatabaseContext context)
    : Repository<WeatherForecast, Guid>(context), IWeatherForecastRepository
{
    public async Task<IEnumerable<WeatherForecast>> GetLatestAsync(int top, QueryOptions options = QueryOptions.None,
        params Expression<Func<WeatherForecast, object>>[] includes)
    {
        return await DbSet
            .ApplyQueryIncludes(includes)
            .ApplyQueryOptions(options)
            .OrderByDescending(x => x.Date)
            .Take(top)
            .ToListAsync();
    }
}