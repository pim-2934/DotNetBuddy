using DotNetBuddy.Application;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Infrastructure.Installers;

/// <summary>
/// Provides an implementation of the <see cref="IInstaller"/> interface to configure filtering behavior
/// for ASP.NET Core applications.
/// </summary>
/// <remarks>
/// This installer customizes MVC behavior by suppressing the automatic ModelState validation filter
/// and adds a custom validation filter, <see cref="ModelStateValidationFilter"/>,
/// to be applied globally to all MVC actions.
/// </remarks>
[InstallPriority(1500000000)]
public class FiltersInstaller : IInstaller
{
    /// <summary>
    /// Configures service registrations to customize ASP.NET Core MVC behaviors.
    /// Suppresses the default ModelState validation filter and globally adds
    /// a custom <see cref="ModelStateValidationFilter"/>.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> used to configure the application's services
    /// and middleware components.
    /// </param>
    public void Install(IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        services.Configure<MvcOptions>(options => { options.Filters.Add<ModelStateValidationFilter>(); });
    }
}