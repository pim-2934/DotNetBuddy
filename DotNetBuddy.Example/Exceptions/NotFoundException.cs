using DotNetBuddy.Application.Exceptions;

namespace DotNetBuddy.Example.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a requested resource cannot be found.
/// </summary>
/// <remarks>
/// This exception is specifically designed for scenarios where a resource is requested by the client
/// but does not exist, returning a standardized HTTP 404 Not Found status code.
/// </remarks>
/// <seealso cref="ResponseException"/>
public class NotFoundException(string message) : ResponseException(message, StatusCodes.Status404NotFound);