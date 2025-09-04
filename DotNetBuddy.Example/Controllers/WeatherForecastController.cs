using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Exceptions;
using DotNetBuddy.Example.Contracts;
using DotNetBuddy.Example.Entities;
using DotNetBuddy.Example.Exceptions;
using DotNetBuddy.Example.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(IExtendedUnitOfWork extendedUnitOfWork, IValidationService validationService)
    : ControllerBase
{
    [HttpPut("UpdateWeatherForecast")]
    public async Task<WeatherForecast> UpdateWeatherForecast(
        Guid id,
        WeatherForecastUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var weatherForecast = await extendedUnitOfWork.WeatherForecasts.GetAsync(
            id,
            cancellationToken: cancellationToken
        ) ?? throw new NotFoundException("Weather forecast not found.");

        validationService.ValidateOrThrow(weatherForecast, dto);
        MapDtoIntoEntity(ref weatherForecast, dto);

        extendedUnitOfWork.WeatherForecasts.UpdateShallow(weatherForecast);
        await extendedUnitOfWork.SaveAsync(cancellationToken);

        return weatherForecast;
    }

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

    // Consider using AutoMapper or similar for DTO-to-entity mapping.
    private static void MapDtoIntoEntity(ref WeatherForecast entity, WeatherForecastUpdateDto dto)
    {
        entity.Date = dto.Date;
        entity.TemperatureC = dto.TemperatureC;
        entity.Summary = dto.Summary;
    }
}