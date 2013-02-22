using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace CodeMercury.Expressions
{
    public static class ExpressionHelper
    {
        public static object Evaluate(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return expression.As<ConstantExpression>().Value;
            else
                return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        public static Expression<Func<TResult>> BindInput<T, TResult>(Expression<Func<T, TResult>> expression, T input)
        {
            var parameterExpression = expression.Parameters.Single();
            var methodCallExpression = expression.Body.As<MethodCallExpression>();
            var visitor = new BindParameterExpressionVisitor
            {
                ParameterExpression = parameterExpression,
                ParameterValue = input
            };
            return Expression.Lambda<Func<TResult>>(visitor.Visit(methodCallExpression));
        }
    }
}
