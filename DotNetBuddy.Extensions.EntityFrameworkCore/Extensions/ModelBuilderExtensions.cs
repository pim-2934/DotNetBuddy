using System.Linq.Expressions;
using DotNetBuddy.Domain;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ModelBuilder"/> class to facilitate additional configurations during model creation.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures query filters for all entities implementing <see cref="ISoftDeletableEntity{TKey}"/> to exclude soft-deleted records.
    /// </summary>
    /// <param name="modelBuilder">
    /// The instance of <see cref="ModelBuilder"/> used to configure entity mappings and behaviors.
    /// </param>
    public static void ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            var isSoftDeletable = clrType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISoftDeletableEntity<>));

            if (!isSoftDeletable)
                continue;

            var interfaceType = clrType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISoftDeletableEntity<>));
            var deletedAtProperty = interfaceType.GetProperty(nameof(ISoftDeletableEntity<object>.DeletedAt));
            var propInfo = clrType.GetProperty(deletedAtProperty!.Name);
            if (propInfo == null) return;

            var param = Expression.Parameter(clrType, "e");
            var deletedAtProp = Expression.Property(param, propInfo);
            var nullValue = Expression.Constant(null, typeof(DateTime?));
            var condition = Expression.Equal(deletedAtProp, nullValue);
            var lambda = Expression.Lambda(condition, param);

            modelBuilder.Entity(clrType).HasQueryFilter(lambda);
        }
    }
}