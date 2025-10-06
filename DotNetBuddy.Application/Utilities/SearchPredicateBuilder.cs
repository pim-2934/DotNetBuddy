using System.Linq.Expressions;
using DotNetBuddy.Domain.Attributes;

namespace DotNetBuddy.Application.Utilities;

/// <summary>
/// Builds search predicates for entities based on properties marked with <see cref="SearchableAttribute"/>.
/// Supports direct string properties, single navigation properties and collection navigations.
/// </summary>
public static class SearchPredicateBuilder
{
    /// <summary>
    /// Builds a search predicate expression for the given entity type and search term.
    /// If the search term is null or whitespace, the method returns null.
    /// Supports optional relation searching for single and collection navigation properties.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which the predicate is built.</typeparam>
    /// <param name="searchTerm">The term to search for in the entity properties. Null or whitespace results in no predicate.</param>
    /// <param name="searchRelations">Indicates whether to include related entities in the search. Default is false.</param>
    /// <returns>An expression representing the search predicate, or null if the search term is empty.</returns>
    public static Expression<Func<TEntity, bool>>? Build<TEntity>(string searchTerm, bool searchRelations = false)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return null;

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression? orChain = null;

        orChain = OrElse(orChain, BuildDirectPropertiesPredicate(typeof(TEntity), parameter, searchTerm));

        if (searchRelations)
        {
            orChain = OrElse(orChain, BuildSingleNavigationPredicate(typeof(TEntity), parameter, searchTerm));
            orChain = OrElse(orChain, BuildCollectionNavigationPredicate(typeof(TEntity), parameter, searchTerm));
        }

        return Expression.Lambda<Func<TEntity, bool>>(orChain, parameter);
    }

    private static Expression? BuildDirectPropertiesPredicate(Type entityType, ParameterExpression parameter,
        string term)
    {
        return GetSearchableStringProperties(entityType)
            .Select(prop => Expression.Property(parameter, prop))
            .Select(member => ContainsOnMember(member, term))
            .Aggregate<Expression?, Expression?>(null, OrElse);
    }

    private static Expression? BuildSingleNavigationPredicate(Type entityType, ParameterExpression parameter,
        string term)
    {
        var singleNavProps = entityType
            .GetProperties()
            .Where(p => p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsArray &&
                        !IsCollectionType(p.PropertyType));

        return (from nav in singleNavProps
                let relatedType = nav.PropertyType
                let relatedSearchables = GetSearchableStringProperties(relatedType)
                where relatedSearchables.Any()
                let relatedInstance = Expression.Property(parameter, nav)
                let notNull = Expression.NotEqual(relatedInstance, Expression.Constant(null, nav.PropertyType))
                from relProp in relatedSearchables
                let relatedMember = Expression.Property(relatedInstance, relProp)
                let contains = ContainsOnMember(relatedMember, term)
                select Expression.AndAlso(notNull, contains))
            .Aggregate<BinaryExpression, Expression?>(null, OrElse);
    }

    private static Expression? BuildCollectionNavigationPredicate(Type entityType, ParameterExpression parameter,
        string term)
    {
        Expression? orChain = null;

        var collectionProps = entityType
            .GetProperties()
            .Where(p => IsCollectionType(p.PropertyType));

        foreach (var nav in collectionProps)
        {
            if (!TryGetElementType(nav.PropertyType, out var elementType))
                continue;

            var relatedSearchables = GetSearchableStringProperties(elementType).ToArray();
            if (relatedSearchables.Length == 0)
                continue;

            var collectionMember = Expression.Property(parameter, nav);
            var notNull = Expression.NotEqual(collectionMember, Expression.Constant(null, nav.PropertyType));

            orChain = (from relProp in relatedSearchables
                       let item = Expression.Parameter(elementType, "item")
                       let itemMember = Expression.Property(item, relProp)
                       let contains = ContainsOnMember(itemMember, term)
                       select Expression.Lambda(contains, item)
                    into predicate
                       select BuildAnyCall(collectionMember, elementType, predicate)
                    into any
                       select Expression.AndAlso(notNull, any))
                .Aggregate(orChain, OrElse);
        }

        return orChain;
    }

    private static IEnumerable<System.Reflection.PropertyInfo> GetSearchableStringProperties(Type type)
    {
        return type
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string) &&
                        p.GetCustomAttributes(typeof(SearchableAttribute), true).Length != 0);
    }

    private static Expression ContainsOnMember(Expression memberExpression, string term)
    {
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
        var termConst = Expression.Constant(term);
        return Expression.Call(memberExpression, containsMethod, termConst);
    }

    private static Expression OrElse(Expression? left, Expression? right)
    {
        if (left == null) return right ?? left!;
        if (right == null) return left;

        return Expression.OrElse(left, right);
    }

    private static bool TryGetElementType(Type collectionType, out Type elementType)
    {
        if (collectionType.IsArray)
        {
            elementType = collectionType.GetElementType()!;
            return true;
        }

        if (collectionType.IsGenericType)
        {
            var args = collectionType.GetGenericArguments();
            if (args.Length == 1)
            {
                elementType = args[0];
                return true;
            }
        }

        elementType = null!;
        return false;
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

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(IEnumerable<>) ||
                genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IList<>))
            {
                return true;
            }
        }

        return type.GetInterfaces().Any(i =>
            i.IsGenericType &&
            (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
             i.GetGenericTypeDefinition() == typeof(ICollection<>) ||
             i.GetGenericTypeDefinition() == typeof(IList<>)));
    }
}