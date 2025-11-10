using System.Text.Json;
using DotNetBuddy.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace DotNetBuddy.Infrastructure.Middleware;

/// <summary>
/// Middleware component for handling exceptions and generating appropriate HTTP responses.
/// </summary>
/// <remarks>
/// The <c>ExceptionMiddleware</c> intercepts exceptions of type <c>ResponseException</c> thrown
/// during the request pipeline, converts them into structured problem details responses,
/// and sends them back to the client with the appropriate HTTP status code and content type.
/// It formats the response as a JSON object conforming to the "application/problem+json"
/// specification. Additional metadata including request instance and optional trace
/// identifiers are added to the response for enhanced debugging purposes.
/// </remarks>
public class ExceptionMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Handles incoming HTTP requests, invokes the next middleware in the pipeline, and processes exceptions
    /// of type <see cref="ResponseException"/> to generate appropriate HTTP responses.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current HTTP request.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of the middleware logic.</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (ResponseException ex)
        {
            httpContext.Response.StatusCode = ex.StatusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Title = ex.GetTranslationKey(),
                Detail = ex.Detail,
                Status = ex.StatusCode,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",

                Extensions =
                {
                    ["metadata"] = ex.GetMetadata(),
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