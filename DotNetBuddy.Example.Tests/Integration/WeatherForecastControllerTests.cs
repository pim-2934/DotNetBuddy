﻿using System.Net;
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
    public async Task GetException_ReturnsTeapotStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/WeatherForecast/GetException");

        // Assert
        Assert.Equal(418, (int)response.StatusCode);
    }

    [Fact]
    public async Task GetException_ReturnsExpectedErrorMessage()
    {
        // Act
        var response = await _client.GetAsync("/WeatherForecast/GetException");
        var content = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<ProblemDetails>(content);

        // Assert
        Assert.NotNull(error);
        Assert.Equal("Test", error.Detail);
        Assert.Equal("This is a test exception", error.Title);
        Assert.Equal(418, error.Status);
    }
}