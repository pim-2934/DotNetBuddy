using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using DotNetBuddy.Domain;
using DotNetBuddy.Example.Contracts;
using DotNetBuddy.Example.Entities;

namespace DotNetBuddy.Example.Validators;

public class WeatherForecastUpdateValidator : IValidator<WeatherForecast, WeatherForecastUpdateDto>
{
    public async IAsyncEnumerable<ValidationResult> ValidateAsync(WeatherForecast source,
        WeatherForecastUpdateDto input, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (source.TemperatureC < input.TemperatureC)
            yield return new ValidationResult("Temperature cannot be greater than the original value.",
                [nameof(input.TemperatureC)]);

        if (source.Summary == input.Summary)
            yield return new ValidationResult("Summary cannot be the same as the original value.",
                [nameof(input.Summary)]);

        await Task.CompletedTask;
    }
}