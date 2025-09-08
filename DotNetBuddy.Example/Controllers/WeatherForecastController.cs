using DotNetBuddy.Example.Exceptions;
using DotNetBuddy.Example.Models;
using DotNetBuddy.Example.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(IExtendedUnitOfWork extendedUnitOfWork) : ControllerBase
{
    [HttpGet("GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast(CancellationToken cancellationToken = default)
    {
        return await extendedUnitOfWork.WeatherForecasts.GetRangeAsync(x => x
                .Take(5)
                .Include(y => y.Location),
            cancellationToken: cancellationToken
        );
    }

    [HttpGet("GetWeatherForecast/{id:guid}")]
    public async Task<WeatherForecast> GetWeatherForecast(Guid id, CancellationToken cancellationToken = default)
    {
        var weatherForecast = await extendedUnitOfWork.WeatherForecasts.GetAsync(
            id,
            x => x.Include(y => y.Location),
            cancellationToken: cancellationToken
        ) ?? throw new NotFoundException("Weather forecast not found.");

        return weatherForecast;
    }

    [HttpPost]
    public async Task<WeatherForecast> Create(
        WeatherForecast weatherForecast,
        CancellationToken cancellationToken = default)
    {
        await extendedUnitOfWork.WeatherForecasts.AddAsync(weatherForecast, cancellationToken);
        await extendedUnitOfWork.SaveAsync(cancellationToken);

        return weatherForecast;
    }

    [HttpGet("Crash")]
    public Task Crash()
    {
        throw new ApplicationException("Crash and Burn");
    }
}