using System.Net;
using DotNetBuddy.Application.Utilities;
using DotNetBuddy.Example.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DotNetBuddy.Example.Tests.Integration;

public class WeatherForecastControllerTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetWeatherForecast_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/WeatherForecast/GetWeatherForecast");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsWeatherForecasts()
    {
        // Act
        var response = await _client.GetAsync("/WeatherForecast/GetWeatherForecast");
        var content = await response.Content.ReadAsStringAsync();
        var forecasts = JsonConvert.DeserializeObject<List<WeatherForecast>>(content);

        // Assert
        Assert.NotNull(forecasts);
        Assert.NotEmpty(forecasts);
        Assert.Equal(5, forecasts.Count); // No soft-deleted entities
    }

    [Fact]
    public async Task GetWeatherForecastById_WithValidId_ReturnsSuccessStatusCode()
    {
        // Arrange
        var validId = BuddyUtils.GenerateDeterministicGuid("Weather Forecast 1");

        // Act
        var response = await _client.GetAsync($"/WeatherForecast/GetWeatherForecast/{validId}");

        // Assert
        Assert.True(response.StatusCode is HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWeatherForecastById_WithValidId_ReturnsWeatherForecast()
    {
        // Arrange
        var validId = BuddyUtils.GenerateDeterministicGuid("Weather Forecast 1");

        // Act
        var response = await _client.GetAsync($"/WeatherForecast/GetWeatherForecast/{validId}");

        var content = await response.Content.ReadAsStringAsync();
        var forecast = JsonConvert.DeserializeObject<WeatherForecast>(content);

        // Assert
        Assert.NotNull(forecast);
        Assert.Equal(validId, forecast.Id);
    }

    [Fact]
    public async Task GetWeatherForecastById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/WeatherForecast/GetWeatherForecast/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetWeatherForecastById_WithInvalidId_ReturnsNotFoundError()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/WeatherForecast/GetWeatherForecast/{invalidId}");
        var content = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<ProblemDetails>(content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal("NotFound", error.Title);
    }

    [Fact]
    public async Task Crash_ReturnsInternalServerError()
    {
        // Act
        var response = await _client.GetAsync("/WeatherForecast/Crash");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Crash_ReturnsErrorResponse()
    {
        // Act
        var response = await _client.GetAsync("/WeatherForecast/Crash");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotEmpty(content);
        // The exact content depends on your error handling middleware
        // You may need to adjust this assertion based on your error response format
    }
}