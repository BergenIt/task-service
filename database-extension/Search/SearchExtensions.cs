
using System.Linq.Expressions;
using System.Reflection;

namespace DatabaseExtension.Search;

public static class SearchExtensions
{
    public const string Splitter = "@splitter";

    public static IEnumerable<T> Search<T>(this IEnumerable<T> query, IEnumerable<SearchFilter>? searchDatas)
    {
        return query.AsQueryable().Search(searchDatas).ToList();
    }

    public static IQueryable<T> Search<T>(this IQueryable<T> query, IEnumerable<SearchFilter>? searchDatas)
    {
        return query.Search(searchDatas?.Select(f => (f.ColumnName, f.Value))?.ToArray());
    }

    public static IQueryable<T> Search<T>(this IQueryable<T> query, (string SearchColumn, string value)[]? searchWhere)
    {
        if (searchWhere?.Any() != true)
        {
            return query;
        }

        foreach ((string SearchColumn, string value) in searchWhere)
        {
            string[] searchColumnProps = SearchColumn.Split(".");

            query = query.Where(SearchWhere<T>(Expression.Parameter(typeof(T), "p"), searchColumnProps, value));
        }

        return query;
    }

    private static Expression<Func<T, bool>> SearchWhere<T>(ParameterExpression parameter, string[] searchColumnProps, string value)
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
                .Invoke(null, new object[] { Expression.Parameter(mainType, "a"), searchColumnProps[(i + 1)..], value })
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
            .MakeGenericMethod(new Type[] { typeof(T), propertyInclude.PropertyType })
            .Invoke(null, new object?[] { propertyPredicate, value });

        return predicate ?? throw new NotImplementedException();
    }

    public static Expression<Func<T, bool>> PredicateBuilder<T, TP>(Expression<Func<T, TP>> propertyExpression, string value)
    {
        Expression<Func<T, bool>> filter;

        //TODO: Or for enum

        if (typeof(TP).IsEnum)
        {
            IEnumerable<object?> enumValues = value.Split(Splitter)
                .Where(v => Enum.TryParse(typeof(TP), v, true, out _))
                .Select(v => Enum.Parse(typeof(TP), v, true));

            filter = entity => propertyExpression.CallVisitor()(entity) != null &&
                enumValues.Contains(propertyExpression.CallVisitor()(entity));

            return filter.VisitorMarker();
        }

        IEnumerable<string> values = value.Split(Splitter).Select(v => v.ToLowerInvariant().Trim());

        Expression<Func<T, TP>> expressionParameter = entity => propertyExpression.CallVisitor()(entity);

        expressionParameter = expressionParameter.VisitorMarker();

        Expression? expressionsOrs = null;

        if (typeof(TP) == typeof(string))
        {
            MethodInfo? containsMethod = typeof(string)
                .GetMethod(nameof(string.Contains), new Type[] { typeof(string) });

            MethodInfo? toLowerMethod = typeof(string)
                .GetMethod(nameof(string.ToLower), Array.Empty<Type>());

            if (toLowerMethod is null || containsMethod is null)
            {
                throw new NotImplementedException();
            }

            foreach (string valueString in values)
            {
                Expression tolowerMethodCall = Expression.Call(
                    expressionParameter.Body,
                    toLowerMethod
                );

                Expression convertMethodCall = Expression.Call(
                    tolowerMethodCall,
                    containsMethod,
                    Expression.Constant(valueString)
                );

                expressionsOrs = expressionsOrs is null ?
                    convertMethodCall :
                    Expression.Or(expressionsOrs, convertMethodCall);
            }
        }
        else
        {
            MethodInfo? toStringMethod = typeof(TP)
                .GetMethod(nameof(object.ToString), Array.Empty<Type>());

            if (toStringMethod is null)
            {
                throw new NotImplementedException();
            }

            Expression toStringMethodCall = Expression.Call(
                expressionParameter.Body,
                toStringMethod
            );

            foreach (string valueString in values)
            {
                Expression equalsCall = Expression.Equal(
                    toStringMethodCall,
                    Expression.Constant(valueString)
                );

                expressionsOrs = expressionsOrs is null
                    ? equalsCall
                    : Expression.Or(expressionsOrs, equalsCall);
            }
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
