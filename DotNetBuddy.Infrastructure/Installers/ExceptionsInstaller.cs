using DotNetBuddy.Application;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetBuddy.Infrastructure.Installers;

/// <summary>
/// Configures and registers HTTP-related services into the dependency
/// injection container, including customization of problem details
/// for enhanced error diagnostics.
/// </summary>
[InstallPriority(2000)]
public class ExceptionsInstaller : IInstaller
{
    /// <summary>
    /// Registers HTTP-related services into the dependency injection container,
    /// including customization of problem details to enhance error diagnostics.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> instance used for registering services.
    /// </param>
    public void Install(IServiceCollection services)
    {
        services.AddProblemDetails
        (
            options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    var httpContext = context.HttpContext;
                    var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

                    if (exception is not Rfc9110Exception rex)
                    {
                        return;
                    }

                    context.ProblemDetails.Title = rex.Title;
                    context.ProblemDetails.Detail = rex.Detail;
                    context.ProblemDetails.Status = rex.StatusCode;
                    context.ProblemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);
                    context.ProblemDetails.Extensions.TryAdd
                    (
                        "traceId",
                        httpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id
                    );

                    httpContext.Response.StatusCode = rex.StatusCode;
                };
            }
        );
    }
}