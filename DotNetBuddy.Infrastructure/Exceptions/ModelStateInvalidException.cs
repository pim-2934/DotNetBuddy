using DotNetBuddy.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetBuddy.Infrastructure.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a model's state is invalid during request validation.
/// </summary>
/// <remarks>
/// This exception is used to encapsulate model validation errors detected in the
/// <see cref="Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary"/> during a request lifecycle.
/// It derives from <see cref="Rfc9110Exception"/> to provide a standardized
/// error message structure and HTTP status code.
/// </remarks>
/// <seealso cref="Rfc9110Exception"/>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary"/>
public class ModelStateInvalidException(ModelStateDictionary modelState) : Rfc9110Exception
(
    string.Join
    (
        ", ",
        modelState
            .Where(x => x.Value is not null && x.Value!.Errors.Any())
            .SelectMany(x => x.Value!.Errors)
            .Select(e => e.ErrorMessage)
    ),
    StatusCodes.Status400BadRequest
);