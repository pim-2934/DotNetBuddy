using System.Linq.Expressions;
using System.Reflection;
using DotNetBuddy.Domain.Attributes;

namespace DotNetBuddy.Application.Utilities;

/// <summary>
/// Builds search predicates for entities based on properties marked with <see cref="SearchableAttribute"/>.
/// </summary>
public static class SearchPredicateBuilder
{
    /// <summary>
    /// Builds a search predicate expression for the given entity type and search term.
    /// Searches properties and navigation properties marked with [Searchable] attribute.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which the predicate is built.</typeparam>
    /// <param name="searchTerm">The term to search for in the entity properties.</param>
    /// <param name="includeRelations">Whether to include navigation properties in the search. Default is true.</param>
    /// <returns>An expression representing the search predicate, or null if the search term is empty.</returns>
    public static Expression<Func<TEntity, bool>>? Build<TEntity>(string searchTerm, bool includeRelations = true)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return null;

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var body = BuildEntitySearchExpression(typeof(TEntity), parameter, searchTerm, includeRelations);

        return body == null ? _ => false : Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private static Expression? BuildEntitySearchExpression(
        Type entityType,
        Expression instance,
        string searchTerm,
        bool includeRelations,
        HashSet<Type>? visitedTypes = null)
    {
        visitedTypes ??= [];
        if (!visitedTypes.Add(entityType))
            return null;

        var result = BuildStringPropertiesSearchExpression(entityType, instance, searchTerm);

        if (includeRelations)
        {
            result = CombineWithOr(
                result,
                BuildNavigationPropertiesSearchExpression(entityType, instance, searchTerm, visitedTypes)
            );
        }

        return result;
    }

    private static Expression? BuildNavigationPropertiesSearchExpression(
        Type entityType,
        Expression instance,
        string searchTerm,
        HashSet<Type> visitedTypes)
    {
        var collectionsSearch = BuildCollectionNavigationsSearchExpression(
            entityType, instance, searchTerm, visitedTypes);

        var singleNavsSearch = BuildSingleNavigationsSearchExpression(
            entityType, instance, searchTerm, visitedTypes);

        return CombineWithOr(collectionsSearch, singleNavsSearch);
    }

    private static Expression? BuildStringPropertiesSearchExpression(
        Type entityType,
        Expression instance,
        string searchTerm)
    {
        var stringProps = GetSearchablePropertiesOfType(entityType, typeof(string));
        if (stringProps.Count == 0)
            return null;

        return stringProps
            .Select(prop => BuildPropertyContainsExpression(instance, prop, searchTerm))
            .Aggregate<Expression, Expression?>(null, CombineWithOr);
    }

    private static Expression? BuildCollectionNavigationsSearchExpression(
        Type entityType,
        Expression instance,
        string searchTerm,
        HashSet<Type> visitedTypes)
    {
        var collectionProps = GetSearchableCollectionProperties(entityType);
        if (collectionProps.Count == 0)
            return null;

        Expression? result = null;

        foreach (var prop in collectionProps)
        {
            Expression? propSearch = BuildCollectionPropertySearchExpression(
                instance, prop, searchTerm, visitedTypes);

            if (propSearch != null)
            {
                result = result == null ? propSearch : Expression.OrElse(result, propSearch);
            }
        }

        return result;
    }

    private static Expression? BuildCollectionPropertySearchExpression(
        Expression instance,
        PropertyInfo collectionProp,
        string searchTerm,
        HashSet<Type> visitedTypes)
    {
        if (!TryGetElementType(collectionProp.PropertyType, out var elementType))
            return null;

        var itemParam = Expression.Parameter(elementType, "item");
        var itemSearchExpression = BuildEntitySearchExpression(
            elementType, itemParam, searchTerm, true, [.. visitedTypes]);

        if (itemSearchExpression == null)
            return null;

        var collectionAccess = Expression.Property(instance, collectionProp);
        var nullCheck = Expression.NotEqual(collectionAccess, Expression.Constant(null, collectionProp.PropertyType));

        var anyLambda = Expression.Lambda(itemSearchExpression, itemParam);
        var anyCall = BuildAnyCall(collectionAccess, elementType, anyLambda);

        return Expression.AndAlso(nullCheck, anyCall);
    }

    private static Expression? BuildSingleNavigationsSearchExpression(
        Type entityType,
        Expression instance,
        string searchTerm,
        HashSet<Type> visitedTypes)
    {
        var singleNavProps = GetSearchableSingleNavigationProperties(entityType);
        if (!singleNavProps.Any())
            return null;

        Expression? result = null;

        foreach (var prop in singleNavProps)
        {
            Expression? propSearch = BuildSingleNavigationPropertySearchExpression(
                instance, prop, searchTerm, visitedTypes);

            if (propSearch != null)
            {
                result = result == null ? propSearch : Expression.OrElse(result, propSearch);
            }
        }

        return result;
    }

    private static Expression? BuildSingleNavigationPropertySearchExpression(
        Expression instance,
        PropertyInfo navProp,
        string searchTerm,
        HashSet<Type> visitedTypes)
    {
        var navAccess = Expression.Property(instance, navProp);
        var nullCheck = Expression.NotEqual(navAccess, Expression.Constant(null, navProp.PropertyType));

        var navSearch = BuildEntitySearchExpression(
            navProp.PropertyType, navAccess, searchTerm, true, [.. visitedTypes]);

        return navSearch == null ? null : Expression.AndAlso(nullCheck, navSearch);
    }

    private static Expression BuildPropertyContainsExpression(
        Expression instance,
        PropertyInfo property,
        string searchTerm)
    {
        var propAccess = Expression.Property(instance, property);
        return BuildStringContainsExpression(propAccess, searchTerm);
    }

    private static Expression BuildStringContainsExpression(Expression stringProperty, string searchTerm)
    {
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
        var searchTermExpr = Expression.Constant(searchTerm);

        // Add null check for the string property
        var nullCheck = Expression.NotEqual(stringProperty, Expression.Constant(null, typeof(string)));
        var contains = Expression.Call(stringProperty, containsMethod, searchTermExpr);

        return Expression.AndAlso(nullCheck, contains);
    }

    private static IReadOnlyCollection<PropertyInfo> GetSearchablePropertiesOfType(
        Type entityType,
        Type propertyType)
    {
        return entityType.GetProperties()
            .Where(p => p.PropertyType == propertyType &&
                        p.GetCustomAttributes(typeof(SearchableAttribute), true).Length > 0)
            .ToArray();
    }

    private static IReadOnlyCollection<PropertyInfo> GetSearchableCollectionProperties(Type entityType)
    {
        return entityType.GetProperties()
            .Where(p => IsCollectionType(p.PropertyType) &&
                        p.GetCustomAttributes(typeof(SearchableAttribute), true).Length > 0)
            .ToArray();
    }

    private static IReadOnlyCollection<PropertyInfo> GetSearchableSingleNavigationProperties(Type entityType)
    {
        return entityType.GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(SearchableAttribute), true).Length > 0 &&
                        p.PropertyType != typeof(string) &&
                        p.PropertyType.IsClass &&
                        !IsCollectionType(p.PropertyType))
            .ToArray();
    }

    private static Expression? CombineWithOr(Expression? left, Expression? right)
    {
        if (left == null) return right;
        if (right == null) return left;

        return Expression.OrElse(left, right);
    }

    private static bool TryGetElementType(Type collectionType, out Type elementType)
    {
        elementType = null!;

        if (collectionType.IsArray)
        {
            elementType = collectionType.GetElementType()!;
            return true;
        }

        if (!collectionType.IsGenericType) return false;

        var args = collectionType.GetGenericArguments();

        if (args.Length != 1) return false;

        elementType = args[0];
        return true;
    }

    private static Expression BuildAnyCall(Expression collection, Type elementType, LambdaExpression predicate)
    {
        var anyMethod = typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
            .MakeGenericMethod(elementType);

        return Expression.Call(anyMethod, collection, predicate);
    }

    private static bool IsCollectionType(Type type)
    {
        if (type == typeof(string))
            return false;

        if (type.IsArray)
            return true;

        if (!type.IsGenericType)
            return type.GetInterfaces().Any(i =>
                i.IsGenericType &&
                (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                 i.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 i.GetGenericTypeDefinition() == typeof(IList<>)));

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(IEnumerable<>) ||
               genericTypeDefinition == typeof(ICollection<>) ||
               genericTypeDefinition == typeof(IList<>) ||
               genericTypeDefinition == typeof(List<>);
    }
}