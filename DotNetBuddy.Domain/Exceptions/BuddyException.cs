namespace DotNetBuddy.Domain.Exceptions;

/// <summary>
/// Represents a custom exception specific to the DotNetBuddy domain.
/// </summary>
/// <remarks>
/// This exception is used for handling errors that occur within the application,
/// providing meaningful and domain-specific exception messages.
/// </remarks>
public class BuddyException(string message) : Exception(message);
