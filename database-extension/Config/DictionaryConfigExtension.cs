
using System.Linq.Expressions;
using System.Reflection;

namespace DatabaseExtension.Config;

public static class DictionaryConfigExtension
{
    public static IConfigProfile<TS, TD> AddCustomRoute<TS, TD, TSP>(this IConfigProfile<TS, TD> routeDictionary, Expression<Func<TS, TSP>> sourseRoute, string distRoute)
        where TS : class
        where TD : class
    {
        string source = sourseRoute.GetPropNameFromExpression();

        routeDictionary.RouteDictionary.Add(source, distRoute);
        return routeDictionary;
    }

    public static IConfigProfile<TS, TD> AddCustomRoute<TS, TSP, TD, TDP>(this IConfigProfile<TS, TD> routeDictionary, Expression<Func<TS, TSP>> sourseRoute, Expression<Func<TD, TDP>> distRoute)
       where TS : class
       where TD : class
    {
        string source = sourseRoute.GetPropNameFromExpression();
        string dist = distRoute.GetPropNameFromExpression();

        routeDictionary.RouteDictionary.Add(source, dist);
        return routeDictionary;
    }
    public static IConfigValueProfile<TD> AddCustomValueRoute<TE, TD>(this IConfigValueProfile<TD> routeDictionary, Expression<Func<TD, TE>> distValueRoute)
       //where E : struct, Enum
       where TD : class
    {
        string dist = distValueRoute.GetPropNameFromExpression();

        routeDictionary.RouteDictionary.Add(typeof(TE), dist);
        return routeDictionary;
    }

    public static string GetPropNameFromExpression<TS, TSP>(this Expression<Func<TS, TSP>> sourseRoute) where TS : class
    {
        if (sourseRoute.Body.NodeType != ExpressionType.Call)
        {
            return sourseRoute.GetPropNameFromParameter();
        }

        string collectionRoute = string.Empty;

        foreach (Expression expression in ((MethodCallExpression)sourseRoute.Body).Arguments)
        {
            collectionRoute += expression.NodeType switch
            {
                ExpressionType.MemberAccess => expression
                        .ToString()
                        .Replace($"{sourseRoute.Parameters.Single().Name}.", string.Empty),

                ExpressionType.Lambda => "." + ((TypeInfo)typeof(DictionaryConfigExtension))
                        .DeclaredMethods
                        .Single(m => m.Name == nameof(GetPropNameFromExpression))
                        .MakeGenericMethod(expression.Type.GenericTypeArguments)
                        .Invoke(null, new object[] { expression }),

                _ => throw new NotImplementedException()
            };
        }

        return collectionRoute;
    }

    private static string GetPropNameFromParameter<TS, TSP>(this Expression<Func<TS, TSP>> sourseRoute) where TS : class
    {
        string? parameterName = sourseRoute.Parameters.Single().Name;

        return sourseRoute.Body.ToString().Replace($"{parameterName}.", string.Empty);
    }

    public static TSP CollectionRoute<TS, TSP>(this IEnumerable<TS> source, Func<TS, TSP> sourseRoute)
    {
        return sourseRoute(source.Single());
        ;
    }
}
