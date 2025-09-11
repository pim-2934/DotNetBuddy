using DotNetBuddy.Application;
using DotNetBuddy.Application.Services;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Infrastructure.Installers;

/// <summary>
/// Represents an installer responsible for registering all validator implementations into the dependency injection container.
/// </summary>
/// <remarks>
/// This installer scans the current application domain for all assemblies, identifies classes that implement the
/// <see cref="IValidator{TSource, TInput}"/> interface, and registers them as scoped services. The registration ensures
/// that validators can be resolved dynamically for their respective interface types.
/// </remarks>
/// <example>
/// During application startup, this installer enables automatic discovery and registration of validators,
/// facilitating the validation logic for various domain entities.
/// </example>
[InstallPriority(1000000000)]
public class ValidatorsInstaller : IInstaller
{
    /// <summary>
    /// Installs services required for validation within the application by discovering and
    /// registering appropriate validator types, and setting up the validation service.
    /// </summary>
    /// <param name="services">A collection of service descriptors used to configure the services for the application.</param>
    public void Install(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var validatorTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t is { IsClass: true, IsAbstract: false, IsPublic: true } &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IValidator<>) ||
                     i.GetGenericTypeDefinition() == typeof(IValidator<,>))
                )
            )
            .ToList();

        foreach (var type in validatorTypes)
        {
            var singleGenericInterfaces = type
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

            foreach (var interfaceType in singleGenericInterfaces)
            {
                services.AddScoped(interfaceType, type);
            }

            var doubleGenericInterfaces = type
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<,>));

            foreach (var interfaceType in doubleGenericInterfaces)
            {
                services.AddScoped(interfaceType, type);
            }
        }

        services.AddScoped<IValidationService, ValidationService>();
    }
}