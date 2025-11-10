namespace DotNetBuddy.Domain.Exceptions;

/// <summary>
/// Represents an exception that occurs when validation fails.
/// </summary>
/// <remarks>
/// This exception is typically thrown when one or more validation errors are encountered during input validation.
/// The exception aggregates all validation errors into a single error message.
/// </remarks>
/// <seealso cref="ResponseException"/>
public class ValidationFailedException(IEnumerable<string> errors) : ResponseException(string.Join(", ", errors), 400);