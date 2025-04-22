namespace DotNetBuddy.Example.Repositories.Interfaces;

public interface IExtendedUnitOfWork : IUnitOfWork
{
    IWeatherForecastRepository WeatherForecasts { get; }
}