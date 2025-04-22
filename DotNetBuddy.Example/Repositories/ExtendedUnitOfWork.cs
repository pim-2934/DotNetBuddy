using BuddyDotNet.Repositories;
using DotNetBuddy.Example.Repositories.Interfaces;

namespace DotNetBuddy.Example.Repositories;

public class ExtendedUnitOfWork(DatabaseContext context) : UnitOfWork<DatabaseContext>(context), IExtendedUnitOfWork
{
    public IWeatherForecastRepository WeatherForecasts { get; } = new WeatherForecastRepository(context);
}