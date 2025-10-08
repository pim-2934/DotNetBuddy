namespace DotNetBuddy.Domain.Exceptions;

/// <summary>
/// Represents the base class for application-specific exceptions that include additional details about the error.
/// </summary>
/// <remarks>
/// The <see cref="ResponseException"/> class provides properties to include an HTTP status code, detailed error message,
/// and optional metadata for exception handling in the application. It also provides a method to retrieve a translation key
/// derived from the exception type.
/// </remarks>
public abstract class ResponseException(string detail, int statusCode, Dictionary<string, object>? metadata = null)
    : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with the current exception.
    /// </summary>
    /// <remarks>
    /// This property indicates the HTTP status code that should be included in the response,
    /// enabling structured error handling and compliance with web standards.
    /// </remarks>
    public int StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the detailed message associated with the exception.
    /// </summary>
    /// <remarks>
    /// This property provides a human-readable explanation of the exception,
    /// which can be included in error responses or logs to provide additional context about the issue.
    /// </remarks>
    public string? Detail { get; } = detail;

    /// <summary>
    /// Gets the metadata associated with the exception.
    /// </summary>
    /// <remarks>
    /// This property provides additional metadata represented as key-value pairs, allowing for
    /// extended context or supplementary information regarding the specific exception instance.
    /// </remarks>
    public Dictionary<string, object>? Metadata { get; } = metadata;

    /// <summary>
    /// Retrieves the translation key for the exception by removing the "Exception"
    /// suffix from the exception type name.
    /// </summary>
    /// <returns>
    /// A string representing the translation key, which is the name of the exception class
    /// without the "Exception" suffix.
    /// </returns>
    public string GetTranslationKey() => GetType().Name.Replace("Exception", string.Empty);
}