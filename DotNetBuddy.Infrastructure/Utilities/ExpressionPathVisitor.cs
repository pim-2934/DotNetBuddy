using System.Linq.Expressions;

namespace DotNetBuddy.Infrastructure.Utilities;

/// <summary>
/// Utility class that extracts property paths from lambda expressions.
/// Used for converting type-safe include expressions to string paths.
/// </summary>
public static class ExpressionPathVisitor
{
    /// <summary>
    /// Extracts a property path from a lambda expression.
    /// Supports nested properties and collection navigation with Select expressions.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TProperty">The property type</typeparam>
    /// <param name="expression">The lambda expression representing the property path</param>
    /// <returns>A string representing the property path (e.g. "Roles.RoleClaims")</returns>
    public static string GetPropertyPath<TEntity, TProperty>(
        Expression<Func<TEntity, TProperty>> expression)
    {
        return GetPropertyPathInternal(expression.Body);
    }

    private static string GetPropertyPathInternal(Expression expression)
    {
        // Handle different expression types for property paths
        return expression switch
        {
            MemberExpression memberExpr =>
                GetMemberExpressionPath(memberExpr),

            MethodCallExpression methodCallExpr when IsSelectMethod(methodCallExpr) =>
                GetSelectMethodPath(methodCallExpr),

            UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unaryExpr =>
                GetPropertyPathInternal(unaryExpr.Operand),

            _ => string.Empty
        };
    }

    private static string GetMemberExpressionPath(MemberExpression memberExpr)
    {
        // For simple member access like x.Property
        if (memberExpr.Expression is ParameterExpression)
            return memberExpr.Member.Name;

        // For nested properties like x.Parent.Property
        var parentPath = GetPropertyPathInternal(memberExpr.Expression!);
        return string.IsNullOrEmpty(parentPath) ? memberExpr.Member.Name : $"{parentPath}.{memberExpr.Member.Name}";
    }

    private static bool IsSelectMethod(MethodCallExpression methodCallExpr)
    {
        return methodCallExpr.Method.Name == "Select" &&
               methodCallExpr.Arguments.Count == 2 &&
               methodCallExpr.Method.DeclaringType is { FullName: not null } &&
               methodCallExpr.Method.DeclaringType.FullName.StartsWith("System.Linq");
    }

    private static string GetSelectMethodPath(MethodCallExpression methodCallExpr)
    {
        // Get the collection path (the part before .Select)
        var collectionPath = GetPropertyPathInternal(methodCallExpr.Arguments[0]);
        if (string.IsNullOrEmpty(collectionPath))
            return string.Empty;

        // Get the property path from the selector lambda
        string propertyPath;
        switch (methodCallExpr.Arguments[1])
        {
            case UnaryExpression { Operand: LambdaExpression lambdaFromUnary }:
                propertyPath = GetPropertyPathInternal(lambdaFromUnary.Body);
                break;
            case LambdaExpression lambdaExpr:
                propertyPath = GetPropertyPathInternal(lambdaExpr.Body);
                break;
            default:
                return collectionPath;
        }

        return string.IsNullOrEmpty(propertyPath) ? collectionPath : $"{collectionPath}.{propertyPath}";
    }
}