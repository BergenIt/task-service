
using System.Linq.Expressions;
using System.Reflection;

namespace DatabaseExtension.Sort;

public static class SortExtension
{
    /// <summary>
    /// Сортировка перечисления
    /// </summary>
    /// <typeparam name="T">Тип сущностей в перечислении</typeparam>
    /// <param name="query">Перечисление для сортировки</param>
    /// <param name="orderBy">Данные по которым необходимо произвести сортировку</param>
    /// <returns></returns>
    public static IEnumerable<T> Sort<T>(this IEnumerable<T> query, IEnumerable<SortFilter>? orderBy)
    {
        return query.AsQueryable().Sort(orderBy).ToList();
    }

    /// <summary>
    /// Сортировка запроса (Ef Core)
    /// </summary>
    /// <typeparam name="T">Тип сущностей в перечислении</typeparam>
    /// <param name="query">Перечисление для сортировки</param>
    /// <param name="orderBy">Данные по которым необходимо произвести сортировку</param>
    /// <returns></returns>
    public static IQueryable<T> Sort<T>(this IQueryable<T> query, IEnumerable<SortFilter>? orderBy)
    {
        if (orderBy?.Any() != true)
        {
            return query;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "p");

        string includableCommand = orderBy.First().IsDescending ? nameof(Queryable.Max) : nameof(Queryable.Min);
        query = Sort(query.AsQueryable(), parameter, orderBy.First().ColumnName, orderBy.First().IsDescending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy), includableCommand);

        if (orderBy.Count() > 1)
        {
            foreach (SortFilter order in orderBy.Skip(1))
            {
                query = Sort(query.AsQueryable(), parameter, order.ColumnName, order.IsDescending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy), order.IsDescending ? nameof(Queryable.Max) : nameof(Queryable.Min));
            }
        }

        return query;
    }

    private static IQueryable<T> Sort<T>(IQueryable<T> query, ParameterExpression parameter, string sortColumn, string command, string includableCommand)
    {
        string[] sortColumnProps = sortColumn.Split(".");

        PropertyInfo propertyInclude = typeof(T).GetProperties()
            .First(x => string.Equals(x.Name, sortColumnProps.First(), StringComparison.CurrentCultureIgnoreCase));

        Expression propertyAccess = Expression.MakeMemberAccess(parameter, propertyInclude);

        MakeProppertyAsset(includableCommand, sortColumnProps[1..], ref propertyInclude, ref propertyAccess);

        LambdaExpression orderByExpression = Expression.Lambda(propertyAccess, parameter);

        MethodInfo methodOrder = ((TypeInfo)typeof(Queryable))
            .DeclaredMethods
            .Single(m => m.Name == command && m.IsGenericMethod && m.GetGenericArguments().Length == 2 && m.GetParameters().Length == 2)
            .MakeGenericMethod(new Type[] { typeof(T), propertyAccess.Type });

        Expression resultExpression = Expression.Call(methodOrder,
                                                      query.Expression,
                                                      Expression.Quote(orderByExpression));

        query = query.Provider.CreateQuery<T>(resultExpression);
        return query;
    }

    private static void MakeProppertyAsset(string includableCommand, string[] sortColumnProps, ref PropertyInfo propertyInclude, ref Expression propertyAccess)
    {
        for (int i = 0; i < sortColumnProps.Length; i++)
        {
            string propName = sortColumnProps[i];
            if (propertyInclude.PropertyType.Namespace != typeof(IEnumerable<>).Namespace && !propertyInclude.PropertyType.IsArray)
            {
                propertyInclude = propertyInclude.PropertyType.GetProperties().First(x => string.Equals(x.Name, propName, StringComparison.CurrentCultureIgnoreCase));
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, propertyInclude);
                continue;
            }

            TypeInfo mainType = (TypeInfo)propertyInclude.PropertyType.GetGenericArguments().Single();
            PropertyInfo includeProp = mainType.DeclaredProperties.First(x => string.Equals(x.Name, propName, StringComparison.CurrentCultureIgnoreCase));

            ParameterExpression parameterExpressionOrder = Expression.Parameter(mainType, "m");

            MethodInfo methodAsQueryable = ((TypeInfo)typeof(Queryable))
                .DeclaredMethods
                .Single(m => m.Name.Contains(nameof(Queryable.AsQueryable)) && m.IsGenericMethod && m.GetGenericArguments().Length == 1)
                .MakeGenericMethod(new Type[] { mainType });

            propertyAccess = Expression.Call(methodAsQueryable, propertyAccess);

            Expression includePropertyAccess = Expression.MakeMemberAccess(parameterExpressionOrder, includeProp);

            MakeProppertyAsset(includableCommand, sortColumnProps[(i + 1)..], ref includeProp, ref includePropertyAccess);

            LambdaExpression lambdaExpression = Expression.Lambda(includePropertyAccess, parameterExpressionOrder);

            MethodInfo methodMax = ((TypeInfo)typeof(Queryable))
                .DeclaredMethods
                .Single(m => m.Name.Contains(includableCommand) && m.IsGenericMethod && m.GetGenericArguments().Length == 2)
                .MakeGenericMethod(new Type[] { mainType, includeProp.PropertyType });

            propertyAccess = Expression.Call(methodMax, propertyAccess, lambdaExpression);
            propertyInclude = includeProp;

            break;
        }
    }
}
