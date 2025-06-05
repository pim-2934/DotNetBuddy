using System.Text.Json;
using DotNetBuddy.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace DotNetBuddy.Infrastructure.Middleware;

/// <summary>
/// Middleware for handling exceptions conforming to the RFC 9110 specification in an ASP.NET Core application.
/// </summary>
/// <remarks>
/// This middleware is responsible for intercepting exceptions of type <see cref="DotNetBuddy.Domain.Exceptions.Rfc9110Exception"/>
/// during the request pipeline and formatting them as HTTP Problem Details responses, as specified by RFC 9110.
/// </remarks>
/// <example>
/// Use this middleware in the request pipeline to provide consistent error response handling for API clients.
/// </example>
/// <param name="next">The next middleware in the request pipeline.</param>
/// <exception cref="DotNetBuddy.Domain.Exceptions.Rfc9110Exception">
/// Thrown when an exception conforming to the RFC 9110 standard occurs in the application.
/// </exception>
public class BuddyExceptionMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Handles exceptions in the HTTP request pipeline by catching <see cref="Rfc9110Exception"/>
    /// and returning structured error responses that comply with RFC 9110 standards.
    /// </summary>
    /// <param name="httpContext">The HTTP context that provides information about the current HTTP request.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous middleware operation. If an
    /// <see cref="Rfc9110Exception"/> is caught, the task writes a structured problem detail
    /// response to the client.
    /// </returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Rfc9110Exception ex)
        {
            httpContext.Response.StatusCode = ex.StatusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Title = ex.Title,
                Detail = ex.Detail,
                Status = ex.StatusCode,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions =
                {
                    ["requestId"] = httpContext.TraceIdentifier
                }
            };

            var activityFeature = httpContext.Features.Get<IHttpActivityFeature>();
            if (activityFeature != null)
            {
                problemDetails.Extensions["traceId"] = activityFeature.Activity.Id;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, options);
        }
    }
}