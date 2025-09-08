namespace DotNetBuddy.Domain.Exceptions;

/// <summary>
/// Represents an exception thrown when validation of an entity or object fails.
/// </summary>
/// <remarks>
/// This exception is specifically used to handle and convey validation errors encountered during operations such as data persistence or object validation.
/// It provides a default message indicating that one or more validation errors have occurred and includes specific details of these errors.
/// The exception conforms to the RFC 9110 standards through its inheritance of the <see cref="Rfc9110Exception"/> class.
/// </remarks>
/// <seealso cref="Rfc9110Exception"/>
public class ValidationException(string detail, int statusCode = 400) : Rfc9110Exception(detail, statusCode);