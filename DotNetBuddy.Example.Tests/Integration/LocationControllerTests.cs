using System.Net;
using System.Text;
using DotNetBuddy.Example.Contracts;
using DotNetBuddy.Example.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DotNetBuddy.Example.Tests.Integration;

public class LocationControllerTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateLocation_WithValidData_ReturnsCreatedLocation()
    {
        // Arrange
        var dto = new LocationCreateDto
        {
            Name = "Test Location"
        };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/Location/CreateLocation", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var location = JsonConvert.DeserializeObject<Location>(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(location);
        Assert.Equal(dto.Name, location.Name);
        Assert.NotEqual(Guid.Empty, location.Id);
    }

    [Fact]
    public async Task CreateLocation_WithNameExceeding255Characters_ReturnsBadRequest()
    {
        // Arrange
        var dto = new LocationCreateDto
        {
            Name = new string('A', 256) // 256 characters, exceeding the 255 limit
        };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/Location/CreateLocation", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problemDetails);
        Assert.Equal("ValidationFailed", problemDetails.Title);
        Assert.Equal(400, problemDetails.Status);
    }
}
