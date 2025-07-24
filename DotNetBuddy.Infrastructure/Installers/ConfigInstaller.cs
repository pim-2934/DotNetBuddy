using System.Reflection;
using DotNetBuddy.Application;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Infrastructure.Installers;

/// <summary>
/// ConfigInstaller is responsible for discovering all types that implement the
/// IConfig interface across loaded assemblies and registering them with the
/// dependency injection container.
/// </summary>
/// <remarks>
/// It registers configurations by identifying classes extending IConfig, extracting
/// their names, binding them to related configuration sections, and validating
/// them both via data annotations and at application startup.
/// </remarks>
[InstallPriority(1000)]
public class ConfigInstaller : IInstaller
{
    /// <summary>
    /// Installs and configures the necessary services within the provided service collection.
    /// This method uses reflection to identify all classes implementing the <c>IConfig</c> interface,
    /// configures them using the dependency injection system, and adds options binding, validation, and startup validation.
    /// </summary>
    /// <param name="services">
    /// The <c>IServiceCollection</c> where services and configurations will be registered.
    /// </param>
    public void Install(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();

        var configuration = provider.GetService<IConfiguration>();
        if (configuration is null)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            
            services.AddSingleton(configuration);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var configTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where
            (
                t =>
                    typeof(IConfig).IsAssignableFrom(t)
                    && t is { IsClass: true, IsAbstract: false, IsPublic: true }
            )
            .ToList();

        var addOptionsMethod = typeof(OptionsServiceCollectionExtensions)
            .GetMethods()
            .First
            (
                m =>
                    m is { Name: "AddOptions", IsGenericMethod: true }
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(IServiceCollection)
            );

        var bindMethod = typeof(OptionsBuilderConfigurationExtensions)
            .GetMethods()
            .First
            (
                m =>
                    m.Name == "Bind"
                    && m.GetParameters().Length == 2
                    && m.GetParameters()[1].ParameterType == typeof(IConfiguration)
            );

        var validateAnnotationsMethod = typeof(OptionsBuilderDataAnnotationsExtensions)
            .GetMethod("ValidateDataAnnotations", BindingFlags.Static | BindingFlags.Public)!;

        var validateOnStartMethod = typeof(OptionsBuilderExtensions)
            .GetMethod("ValidateOnStart", BindingFlags.Static | BindingFlags.Public)!;

        foreach (var configType in configTypes)
        {
            var sectionName = configType.Name.EndsWith("Config")
                ? configType.Name[..^"Config".Length]
                : configType.Name;

            var section = configuration.GetSection(sectionName);

            // .AddOptions<T>()
            var addOptionsGeneric = addOptionsMethod.MakeGenericMethod(configType);
            var optionsBuilder = addOptionsGeneric.Invoke
            (
                null,
                [
                    services
                ]
            );

            // .Bind(section)
            var bindGeneric = bindMethod.MakeGenericMethod(configType)
                .Invoke
                (
                    null,
                    [
                        optionsBuilder!, section
                    ]
                );

            // .ValidateDataAnnotations()
            var validateGeneric = validateAnnotationsMethod.MakeGenericMethod(configType)
                .Invoke
                (
                    null,
                    [
                        bindGeneric!
                    ]
                );

            // .ValidateOnStart()
            validateOnStartMethod.MakeGenericMethod(configType)
                .Invoke
                (
                    null,
                    [
                        validateGeneric!
                    ]
                );
        }
    }
}