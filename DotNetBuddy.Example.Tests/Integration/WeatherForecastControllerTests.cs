using System.Net;
using System.Text;
using DotNetBuddy.Application.Utilities;
using DotNetBuddy.Example.Contracts;
using DotNetBuddy.Example.Entities;
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
        Assert.Equal("NotFound", error.Detail);
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
    }

    [Fact]
    public async Task UpdateWeatherForecast_WithValidData_ReturnsUpdatedEntity()
    {
        // Arrange
        var id = BuddyUtils.GenerateDeterministicGuid("Weather Forecast 1");
        var dto = new WeatherForecastUpdateDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            // validator requires input temperature not greater than original; original data likely around range; choose smaller
            TemperatureC = -100,
            Summary = "Updated summary"
        };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/WeatherForecast/UpdateWeatherForecast?id={id}", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var updated = JsonConvert.DeserializeObject<WeatherForecast>(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updated);
        Assert.Equal(id, updated.Id);
        Assert.Equal(dto.Summary, updated.Summary);
        Assert.Equal(dto.TemperatureC, updated.TemperatureC);
        Assert.Equal(dto.Date, updated.Date);
    }

    [Fact]
    public async Task UpdateWeatherForecast_WithHigherTemperature_ReturnsBadRequest()
    {
        // Arrange
        var id = BuddyUtils.GenerateDeterministicGuid("Weather Forecast 1");
        // To violate validator, set temperature higher than original. Use a very high value.
        var dto = new WeatherForecastUpdateDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            TemperatureC = 999,
            Summary = "Different summary"
        };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/WeatherForecast/UpdateWeatherForecast?id={id}", content);
        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonConvert.DeserializeObject<ProblemDetails>(body);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("One or more validation errors occurred.", problem.Title);
        Assert.Equal(400, problem.Status);
    }

    [Fact]
    public async Task UpdateWeatherForecast_WithSameSummary_ReturnsBadRequest()
    {
        // Arrange: first fetch existing to get its current summary
        var id = BuddyUtils.GenerateDeterministicGuid("Weather Forecast 1");
        var get = await _client.GetAsync($"/WeatherForecast/GetWeatherForecast/{id}");
        var existingJson = await get.Content.ReadAsStringAsync();
        var existing = JsonConvert.DeserializeObject<WeatherForecast>(existingJson)!;

        var dto = new WeatherForecastUpdateDto
        {
            Date = existing.Date,
            TemperatureC = existing.TemperatureC - 1, // not greater
            Summary = existing.Summary // same summary -> invalid per validator
        };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/WeatherForecast/UpdateWeatherForecast?id={id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateWeatherForecast_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new WeatherForecastUpdateDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            TemperatureC = -50,
            Summary = "New"
        };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/WeatherForecast/UpdateWeatherForecast?id={id}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}