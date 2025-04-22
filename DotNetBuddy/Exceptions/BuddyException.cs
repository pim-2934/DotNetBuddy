using Microsoft.AspNetCore.Http;

namespace BuddyDotNet.Exceptions;

/// <summary>
/// Represents a custom exception specific to the BuddyDotNet platform.
/// </summary>
/// <remarks>
/// The BuddyException class extends the Rfc9110Exception and provides a formalized method
/// for reporting errors in compliance with the problem detail standards defined by RFC 9110.
/// It is typically used to communicate application-specific errors in a structured manner.
/// </remarks>
/// <example>
/// This exception can be thrown when application-specific errors occur, providing meaningful
/// context such as a human-readable title, error message, and the relevant HTTP status code.
/// </example>
public class BuddyException(string title, string detail, int statusCode = StatusCodes.Status400BadRequest)
    : Rfc9110Exception(title, detail, statusCode);