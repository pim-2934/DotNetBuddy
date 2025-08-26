using DotNetBuddy.Domain.Exceptions;
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
        var weatherForecast = await extendedUnitOfWork.WeatherForecasts.GetAsync(id, cancellationToken: cancellationToken);

        return weatherForecast ??
               throw new BuddyHttpException("NotFound", "Weather forecast not found.", StatusCodes.Status404NotFound);
    }

    [HttpGet("GetException")]
    public Task GetException()
    {
        throw new BuddyHttpException("This is a test exception", "Test", StatusCodes.Status418ImATeapot);
    }

    [HttpGet("Crash")]
    public Task Crash()
    {
        throw new ApplicationException("Crash and Burn");
    }
}