using DotNetBuddy.Domain;
using DotNetBuddy.Example.Models;
using DotNetBuddy.Example.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotNetBuddy.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(IExtendedUnitOfWork extendedUnitOfWork) : ControllerBase
{
    [HttpGet("GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast()
    {
        return await extendedUnitOfWork.WeatherForecasts.GetLatestAsync(5, includes: x => x.Location!);
    }

    [HttpGet("GetWeatherForecast/{id:guid}")]
    public async Task<WeatherForecast> GetWeatherForecast(Guid id)
    {
        var weatherForecast = await extendedUnitOfWork.WeatherForecasts.GetAsync(id);

        if (weatherForecast is null)
            throw new BuddyException("NotFound", "Weather forecast not found.", StatusCodes.Status404NotFound);

        return weatherForecast;
    }

    [HttpGet("GetException")]
    public Task GetException()
    {
        throw new BuddyException("This is a test exception", "Test", StatusCodes.Status418ImATeapot);
    }

    [HttpGet("Crash")]
    public Task Crash()
    {
        throw new ApplicationException("Crash and Burn");
    }
}