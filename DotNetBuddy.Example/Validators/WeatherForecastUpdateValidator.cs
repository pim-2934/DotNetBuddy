using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Domain;
using DotNetBuddy.Example.Contracts;
using DotNetBuddy.Example.Entities;

namespace DotNetBuddy.Example.Validators;

public class WeatherForecastUpdateValidator : IValidator<WeatherForecast, WeatherForecastUpdateDto>
{
    public IEnumerable<ValidationResult> Validate(WeatherForecast source, WeatherForecastUpdateDto input)
    {
        if (source.TemperatureC < input.TemperatureC)
            yield return new ValidationResult("Temperature cannot be greater than the original value.",
                [nameof(input.TemperatureC)]);

        if (source.Summary == input.Summary)
            yield return new ValidationResult("Summary cannot be the same as the original value.",
                [nameof(input.Summary)]);
    }
}