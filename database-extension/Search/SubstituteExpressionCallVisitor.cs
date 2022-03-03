
using System.Linq.Expressions;
using System.Reflection;

namespace DatabaseExtension.Search;

/// <summary>
/// Визитор вызываемыйй маркером для CallVisitor
/// </summary>
public class SubstituteExpressionCallVisitor : ExpressionVisitor
{
    private readonly MethodInfo _markerDesctiprion;

    public SubstituteExpressionCallVisitor()
    {
        _markerDesctiprion = typeof(VisitorExtension)
            .GetMethod(nameof(VisitorExtension.CallVisitor))?
            .GetGenericMethodDefinition()
            ?? throw new NotImplementedException();
    }

    protected override Expression VisitInvocation(InvocationExpression node)
    {
        bool isMarkerCall = node.Expression.NodeType == ExpressionType.Call && IsMarker((MethodCallExpression)node.Expression);

        if (isMarkerCall)
        {
            LambdaExpression expressionToVisit = Unwrap((MethodCallExpression)node.Expression);

            ParameterVisitor parameterReplacer = new(
                node.Arguments.ToArray(),
                expressionToVisit);

            Expression expressionReplace = parameterReplacer.ExpressionReplace();

            Expression expressionVisit = base.Visit(expressionReplace);

            return expressionVisit;
        }

        return base.VisitInvocation(node);
    }

    private static LambdaExpression Unwrap(MethodCallExpression node)
    {
        Delegate compileLambda = Expression
            .Lambda(node.Arguments[0])
            .Compile();

        object? lambdaExpression = compileLambda.DynamicInvoke();

        if (lambdaExpression is not LambdaExpression result)
        {
            throw new InvalidOperationException();
        }

        return result;
    }

    private bool IsMarker(MethodCallExpression node)
    {
        return node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == _markerDesctiprion;
    }
}

