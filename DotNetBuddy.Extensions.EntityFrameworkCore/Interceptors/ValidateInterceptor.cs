using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DotNetBuddy.Extensions.EntityFrameworkCore.Interceptors;

/// <summary>
/// Intercepts DbContext SaveChanges or SaveChangesAsync operations to validate entities
/// implementing the <see cref="System.ComponentModel.DataAnnotations.IValidatableObject"/> interface.
/// </summary>
/// <remarks>
/// This interceptor ensures that entities being added or modified within the DbContext are
/// validated against the rules defined in their <see cref="System.ComponentModel.DataAnnotations.IValidatableObject"/>
/// implementation. If validation fails, a <see cref="System.ComponentModel.DataAnnotations.ValidationException"/>
/// is thrown, preventing the operation from proceeding.
/// </remarks>
/// <example>
/// To use the interceptor, it should be registered with the DbContext's options.
/// The interceptor will automatically be invoked during SaveChanges or SaveChangesAsync calls.
/// </example>
public class ValidateInterceptor : SaveChangesInterceptor
{
    /// Handles the event triggered before changes are saved to the database. Validates entities
    /// that are being added or modified to ensure they meet the required validation rules. If any
    /// entity fails validation, a ValidationException is thrown.
    /// <param name="eventData">
    /// The event data that contains information about the current save operation context.
    /// </param>
    /// <param name="result">
    /// The interception result, which can be modified or returned unchanged based on additional logic.
    /// </param>
    /// <returns>
    /// Returns the interception result to continue with the save operation or modify its result.
    /// </returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ValidateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// Asynchronously intercepts the process of saving changes to the database, validates entities
    /// before persisting, and allows modifying or overriding the saving process.
    /// <param name="eventData">
    /// The context-specific information related to the event, including the DbContext instance and
    /// information about entities being saved.
    /// </param>
    /// <param name="result">
    /// The result of the save operation before it is potentially intercepted or modified.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional token to monitor for cancellation requests, passed to handle asynchronous operations.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an instance of
    /// <see cref="InterceptionResult{TResult}"/> that allows modifying or suppressing the result of the save operation.
    /// </returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ValidateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// Validates entities in the given DbContext to ensure all added or modified entities
    /// meet their validation requirements. Throws a ValidationException if any entity fails validation.
    /// <param name="dbContext">The DbContext containing entities to validate. If null, no validation occurs.</param>
    private static void ValidateEntities(DbContext? dbContext)
    {
        if (dbContext == null) return;

        var entities = dbContext.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var entity in entities)
        {
            if (entity is not IValidatableObject validatableObject) continue;
            var validationContext = new ValidationContext(validatableObject);
            var validationResults = new List<ValidationResult>();

            if (Validator.TryValidateObject(validatableObject, validationContext, validationResults, true)) continue;
            var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
            throw new ValidationException($"Entity validation failed: {errors}");
        }
    }
}