using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Application.Services;
using DotNetBuddy.Domain;
using DotNetBuddy.Tests.Entities;
using DotNetBuddy.Tests.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Tests.Unit;

public class ValidationServiceTests
{
    private static ServiceProvider BuildProviderWithValidator()
    {
        var services = new ServiceCollection();
        services.AddScoped<IValidator<ComplexEntity, ComplexEntity>, ComplexEntityUpdateValidator>();
        services.AddScoped<IValidationService, ValidationService>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Validate_WhenNoValidatorRegistered_ReturnsEmptyCollection()
    {
        // Arrange: build provider without registering any validator
        var services = new ServiceCollection();
        services.AddScoped<IValidationService, ValidationService>();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IValidationService>();

        var source = new ComplexEntity { Id = Guid.NewGuid(), BaseValue = 10, BelowBaseValue = 5, UnchangeableValue = 7 };
        var input = new ComplexEntity { Id = source.Id, BaseValue = 1, BelowBaseValue = 2, UnchangeableValue = 8 };

        // Act
        var results = new List<ValidationResult>();
        await foreach (var result in service.ValidateAsync(source, input))
        {
            results.Add(result);
        }

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task Validate_WithRegisteredValidator_ReturnsExpectedFailures()
    {
        // Arrange
        await using var provider = BuildProviderWithValidator();
        var service = provider.GetRequiredService<IValidationService>();

        var source = new ComplexEntity { Id = Guid.NewGuid(), BaseValue = 10, BelowBaseValue = 5, UnchangeableValue = 7 };
        // Violates two rules:
        // - UnchangeableValue modified
        // - BelowBaseValue > BaseValue
        var input = new ComplexEntity { Id = source.Id, BaseValue = 3, BelowBaseValue = 9, UnchangeableValue = 8 };

        // Act
        var results = new List<ValidationResult>();
        await foreach (var result in service.ValidateAsync(source, input))
        {
            results.Add(result);
        }

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.ErrorMessage == "UnchangeableValue cannot be modified once set." && r.MemberNames.Contains(nameof(source.UnchangeableValue)));
        Assert.Contains(results, r => r.ErrorMessage == "Bar is not allowed to be greater than Foo." && r.MemberNames.Contains(nameof(input.BelowBaseValue)));
    }

    [Fact]
    public async Task Validate_WithValidInput_ReturnsEmpty()
    {
        // Arrange
        await using var provider = BuildProviderWithValidator();
        var service = provider.GetRequiredService<IValidationService>();

        var source = new ComplexEntity { Id = Guid.NewGuid(), BaseValue = 10, BelowBaseValue = 5, UnchangeableValue = 7 };
        var input = new ComplexEntity { Id = source.Id, BaseValue = 9, BelowBaseValue = 4, UnchangeableValue = 7 };

        // Act
        var results = new List<ValidationResult>();
        await foreach (var result in service.ValidateAsync(source, input))
        {
            results.Add(result);
        }

        // Assert
        Assert.Empty(results);
    }
}
