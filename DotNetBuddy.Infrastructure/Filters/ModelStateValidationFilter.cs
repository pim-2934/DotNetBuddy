using DotNetBuddy.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetBuddy.Infrastructure.Filters;

/// <summary>
/// An action filter that validates the `ModelState` before executing the action method.
/// </summary>
/// <remarks>
/// This filter checks the `ModelState` for validity during the `OnActionExecuting` phase of the request pipeline.
/// If the `ModelState` is invalid, it throws a <see cref="ValidationFailedException"/>,
/// which encapsulates the validation errors.
/// </remarks>
/// <example>
/// To use this filter globally, ensure it is added to the application's MVC options via a filter installer
/// or other configuration mechanism.
/// </example>
/// <see cref="Microsoft.AspNetCore.Mvc.Filters.IActionFilter"/>
/// <see cref="ValidationFailedException"/>
public class ModelStateValidationFilter : IActionFilter
{
    /// <summary>
    /// Invoked before an action method is executed to validate the `ModelState`.
    /// </summary>
    /// <param name="context">
    /// The context associated with the executing action, containing information
    /// such as the `ModelState` and the HTTP request.
    /// </param>
    /// <exception cref="ValidationFailedException">
    /// Thrown if the `ModelState` is invalid, encapsulating the details of the validation errors.
    /// </exception>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            throw new ValidationFailedException(context.ModelState
                .Where(x => x.Value is not null && x.Value!.Errors.Any())
                .SelectMany(x => x.Value!.Errors)
                .Select(e => e.ErrorMessage)
            );
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // no-op
    }
}