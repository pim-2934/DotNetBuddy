using BuddyDotNet.Exceptions;
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
        return await extendedUnitOfWork.WeatherForecasts.GetAllAsync();
    }

    [HttpGet("GetException")]
    public Task GetException()
    {
        throw new BuddyException("This is a test exception", "Test", StatusCodes.Status418ImATeapot);
    }
}