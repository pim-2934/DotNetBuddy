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
    /// Registers validator implementations in the dependency injection container by discovering all types that implement the
    /// generic <see cref="IValidator{TRequest, TResponse}"/> interface within the application domain's current assemblies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the discovered validators should be registered.</param>
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
                    i.GetGenericTypeDefinition() == typeof(IValidator<,>)
                )
            )
            .ToList();

        foreach (var type in validatorTypes)
        {
            var interfaceTypes = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<,>));

            foreach (var interfaceType in interfaceTypes)
            {
                services.AddScoped(interfaceType, type);
            }
        }

        services.AddScoped<IValidationService, ValidationService>();
    }
}