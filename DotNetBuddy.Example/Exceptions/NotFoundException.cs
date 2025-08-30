using DotNetBuddy.Domain.Exceptions;

namespace DotNetBuddy.Example.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a requested resource cannot be found.
/// </summary>
/// <remarks>
/// This exception is specifically designed for scenarios where a resource is requested by the client
/// but does not exist, returning a standardized HTTP 404 Not Found status code.
/// </remarks>
/// <seealso cref="Rfc9110Exception"/>
public class NotFoundException(string title)
    : Rfc9110Exception(
        title,
        nameof(NotFoundException).Replace("Exception", string.Empty),
        StatusCodes.Status404NotFound
    );