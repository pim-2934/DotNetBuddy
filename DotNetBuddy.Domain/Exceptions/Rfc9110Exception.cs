
namespace DotNetBuddy.Exceptions;

/// <summary>
/// Represents the base exception class that conforms to the RFC 9110 standard for structured error responses.
/// </summary>
/// <remarks>
/// This exception is intended to provide a consistent structure for error responses as defined by HTTP APIs.
/// It includes standard properties for specifying HTTP status codes as well as detailed error information.
/// </remarks>
/// <seealso cref="System.Exception"/>
/// <seealso cref="Microsoft.AspNetCore.Http.StatusCodes"/>
public abstract class Rfc9110Exception(string title, string detail, int statusCode)
    : Exception
{
    /// <summary>
    /// Gets or sets the HTTP status code associated with the exception.
    /// </summary>
    /// <remarks>
    /// This property typically represents the HTTP status code that should be returned in response to the exception,
    /// allowing for alignment with standardized HTTP response handling.
    /// </remarks>
    public int StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the error message or code associated with the exception.
    /// </summary>
    /// <remarks>
    /// This property provides a concise representation of the primary error associated with the exception,
    /// typically used to identify the issue in a human-readable or machine-readable format.
    /// </remarks>
    public string? Detail { get; } = detail;

    /// <summary>
    /// Gets or sets the descriptive message associated with the exception.
    /// </summary>
    /// <remarks>
    /// This property provides a human-readable explanation of the error, offering
    /// additional context or details regarding the exception. It is typically used
    /// to convey meaningful information to the client or end user.
    /// </remarks>
    public string Title { get; } = title;
}