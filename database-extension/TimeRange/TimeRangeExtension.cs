using DatabaseExtension.Search;

using System.Linq.Expressions;
using System.Reflection;

namespace DatabaseExtension.TimeRange;

public static class TimeRangeExtension
{
    /// <summary>
    /// Сортировка перечисления
    /// </summary>
    /// <typeparam name="T">Тип сущностей в перечислении</typeparam>
    /// <param name="query">Перечисление для сортировки</param>
    /// <param name="orderBy">Данные по которым необходимо произвести сортировку</param>
    /// <returns></returns>
    public static IEnumerable<T> FilterByTimeRange<T>(this IEnumerable<T> query, IEnumerable<TimeRangeFilter>? timeRangeFilter)
    {
        return query.AsQueryable().FilterByTimeRange(timeRangeFilter).ToList();
    }

    /// <summary>
    /// Сортировка запроса (Ef Core)
    /// </summary>
    /// <typeparam name="T">Тип сущностей в перечислении</typeparam>
    /// <param name="query">Перечисление для сортировки</param>
    /// <param name="orderBy">Данные по которым необходимо произвести сортировку</param>
    /// <returns></returns>
    public static IQueryable<T> FilterByTimeRange<T>(this IQueryable<T> query, IEnumerable<TimeRangeFilter>? timeRangeFilter)
    {
        if (timeRangeFilter?.Any() != true)
        {
            return query;
        }

        foreach (IGrouping<string, TimeRangeFilter>? group in timeRangeFilter.GroupBy(t => t.ColumnName))
        {
            string[] searchColumnProps = group.Key.Split(".");

            query = query.Where(SearchWhere<T>(Expression.Parameter(typeof(T), "p"), searchColumnProps, group));
        }

        return query;
    }

    private static Expression<Func<T, bool>> SearchWhere<T>(ParameterExpression parameter, string[] searchColumnProps, IEnumerable<TimeRangeFilter> timeRangeFilters)
    {
        PropertyInfo propertyInclude = typeof(T)
            .GetProperties()
            .First(x =>
                string.Equals(x.Name, searchColumnProps.First(), StringComparison.CurrentCultureIgnoreCase)
            );

        Expression expressionProperty = Expression.Property(parameter, propertyInclude);

        for (int i = 0; i < searchColumnProps[1..].Length; i++)
        {
            string propName = searchColumnProps[i + 1];

            if (propertyInclude.PropertyType.Namespace != typeof(IEnumerable<>).Namespace && !propertyInclude.PropertyType.IsArray)
            {
                propertyInclude = propertyInclude.PropertyType.GetProperties().First(x => string.Equals(x.Name, propName, StringComparison.CurrentCultureIgnoreCase));
                expressionProperty = Expression.MakeMemberAccess(expressionProperty, propertyInclude);
                continue;
            }

            TypeInfo mainType = (TypeInfo)propertyInclude.PropertyType.GetGenericArguments().Single();

            PropertyInfo includeProp = mainType.DeclaredProperties.First(x => string.Equals(x.Name, propName, StringComparison.CurrentCultureIgnoreCase));

            MethodInfo methodAsQueryable = ((TypeInfo)typeof(Queryable))
                .DeclaredMethods
                .Single(m => m.Name.Contains(nameof(Queryable.AsQueryable)) && m.IsGenericMethod && m.GetGenericArguments().Length == 1)
                .MakeGenericMethod(new Type[] { mainType });

            expressionProperty = Expression.Call(methodAsQueryable, expressionProperty);

            Expression anyPredicate = (Expression)(((TypeInfo)typeof(SearchExtensions))
                .DeclaredMethods
                .Single(m => m.Name == nameof(SearchWhere))
                .MakeGenericMethod(new Type[] { mainType })
                .Invoke(null, new object[] { Expression.Parameter(mainType, "a"), searchColumnProps[(i + 1)..], timeRangeFilters })
                ?? throw new NotImplementedException());

            MethodInfo methodAny = ((TypeInfo)typeof(Queryable))
                .DeclaredMethods
                .Single(m => m.Name == nameof(Queryable.Any) && m.IsGenericMethod && m.GetParameters().Length == 2)
                .MakeGenericMethod(new Type[] { mainType });

            expressionProperty = Expression.Call(null, methodAny, expressionProperty, anyPredicate);

            object lambdaAny = typeof(Expression)
                .GetMethod(nameof(Expression.Lambda), 1, new Type[] { typeof(Expression), typeof(ParameterExpression[]) })?
                .MakeGenericMethod(new Type[] { typeof(Func<,>).MakeGenericType(new Type[] { typeof(T), typeof(bool) }) })
                .Invoke(null, new object[] { expressionProperty, new ParameterExpression[] { parameter } })
                ?? throw new NotImplementedException();

            return (Expression<Func<T, bool>>)lambdaAny;
        }

        Type typeExpression = typeof(Expression<>)
            .MakeGenericType(new Type[] { typeof(Func<,>).MakeGenericType(new Type[] { typeof(T), propertyInclude.PropertyType }) });

        object? propertyPredicate = typeof(Expression)
            .GetMethod(nameof(Expression.Lambda), 1, new Type[] { typeof(Expression), typeof(ParameterExpression[]) })?
            .MakeGenericMethod(new Type[] { typeof(Func<,>).MakeGenericType(new Type[] { typeof(T), propertyInclude.PropertyType }) })
            .Invoke(null, new object[] { expressionProperty, new ParameterExpression[] { parameter } });

        Expression<Func<T, bool>>? predicate = (Expression<Func<T, bool>>?)typeof(SearchExtensions)
            .GetMethod(nameof(PredicateBuilder))?
            .MakeGenericMethod(new Type[] { typeof(T) })
            .Invoke(null, new object?[] { propertyPredicate, timeRangeFilters });

        return predicate ?? throw new NotImplementedException();
    }

    public static Expression<Func<T, bool>> PredicateBuilder<T>(Expression<Func<T, DateTime>> propertyExpression, IEnumerable<TimeRangeFilter> timeRangeFilters)
    {
        Expression<Func<T, bool>> filter;

        Expression<Func<T, DateTime>> expressionParameter = entity => propertyExpression.CallVisitor()(entity);

        expressionParameter = expressionParameter.VisitorMarker();

        Expression? expressionsOrs = null;

        MethodInfo? compareMethod = typeof(string)
            .GetMethod(nameof(DateTime.Equals), new Type[] { typeof(DateTime) });

        if (compareMethod is null)
        {
            throw new NotImplementedException();
        }

        foreach (TimeRangeFilter rangeFilter in timeRangeFilters)
        {
            ConstantExpression startConstant = Expression.Constant(rangeFilter.StartRange, typeof(DateTime));
            ConstantExpression endConstant = Expression.Constant(rangeFilter.EndRange, typeof(DateTime));

            BinaryExpression startCall = Expression.GreaterThanOrEqual(expressionParameter.Body, startConstant);
            BinaryExpression endCall = Expression.LessThanOrEqual(expressionParameter.Body, endConstant);

            BinaryExpression compareResult = Expression.And(startCall, endCall);

            expressionsOrs = expressionsOrs is null ?
                compareResult :
                Expression.Or(expressionsOrs, compareResult);
        }

        //  TODO: доделать для null
        //  Expression<Func<T, bool>> nullCheckExpression = entity => propertyExpression.CallVisitor()(entity) != null;
        //  nullCheckExpression = nullCheckExpression.VisitorMarker();
        //  Expression and = Expression.AndAlso(nullCheckExpression.Body, expressionsOrs);

        if (expressionsOrs is null)
        {
            throw new NotImplementedException();
        }

        filter = (Expression<Func<T, bool>>)Expression.Lambda(expressionsOrs, expressionParameter.Parameters);

        return filter;
    }
}
