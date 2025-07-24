using DotNetBuddy.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DotNetBuddy.Extensions.EntityFrameworkCore.Interceptors;

/// <summary>
/// Represents a custom EF Core <see cref="SaveChangesInterceptor"/> that applies auditing information to entities
/// implementing <see cref="IAuditableEntity{TKey}"/> during save operations (either add or update).
/// </summary>
/// <remarks>
/// The interceptor is designed to populate or modify audit-specific fields such as creation and modification timestamps
/// for entities that implement the <see cref="IAuditableEntity{TKey}"/> interface. It intercepts both synchronous and asynchronous
/// save operations.
/// </remarks>
public class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Overrides the <see cref="SaveChangesInterceptor.SavingChanges"/> method to apply auditing information
    /// to entities implementing the <see cref="IAuditableEntity{TKey}"/> interface during save operations.
    /// </summary>
    /// <param name="eventData">Contextual information about the operation being intercepted.</param>
    /// <param name="result">The probable result of the current save operation prior to interception.</param>
    /// <returns>An <see cref="InterceptionResult{T}"/> object that allows modifying or suppressing the save operation.</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Overrides the <see cref="SaveChangesInterceptor.SavingChangesAsync"/> method to asynchronously apply auditing information
    /// to entities implementing the <see cref="IAuditableEntity{TKey}"/> interface during save operations.
    /// </summary>
    /// <param name="eventData">Contextual information about the operation being intercepted.</param>
    /// <param name="result">The probable result of the current save operation prior to interception.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing an <see cref="InterceptionResult{T}"/>
    /// that allows modifying or suppressing the save operation asynchronously.</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ApplyAuditInfo(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity.GetType().GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAuditableEntity<>)) 
                && entry.State is EntityState.Added or EntityState.Modified)
            {
                dynamic entity = entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = utcNow;
                        entity.UpdatedAt = utcNow;
                        break;

                    case EntityState.Modified:
                        entity.UpdatedAt = utcNow;
                        entry.Property("CreatedAt").IsModified = false;
                        break;
                }
            }
        }
    }
}