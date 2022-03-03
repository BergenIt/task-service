
using System.Linq.Expressions;

namespace DatabaseExtension.Search;

public static class VisitorExtension
{
    /// <summary>
    /// Является маркером для вызова визитора
    /// </summary>
    /// <typeparam name="TFunc">Оборабатываемое выржаение</typeparam>
    /// <param name="_"></param>
    /// <returns></returns>
    public static TFunc CallVisitor<TFunc>(this Expression<TFunc> _)
    {
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        return default;
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
    }

    /// <summary>
    /// Явно вызывает визитора для выражения отмеченного CallVisitor
    /// </summary>
    /// <typeparam name="TFunc"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static Expression<TFunc> VisitorMarker<TFunc>(this Expression<TFunc> expression)
    {
        return (Expression<TFunc>)new SubstituteExpressionCallVisitor().Visit(expression);
    }
}

