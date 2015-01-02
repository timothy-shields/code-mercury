using CodeMercury.Domain.Models;
using CodeMercury.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Services
{
    public static class InvocationBuilder
    {
        public static Invocation Build(MethodCallExpression expression)
        {
            var @object = GetObjectArgument(expression);
            var invocation = Build(@object, expression);
            return invocation;
        }

        public static Invocation Build(Argument @object, MethodCallExpression expression)
        {
            var method = GetMethod(expression);
            var arguments = GetArguments(expression);
            var invocation = new Invocation(@object, method, arguments);
            return invocation;
        }

        public static Invocation Build(Expression<Action> expression)
        {
            return Build((MethodCallExpression)expression.Body);
        }

        public static Invocation Build<T>(Argument @object, Expression<Action<T>> expression)
        {
            return Build(@object, (MethodCallExpression)expression.Body);
        }

        public static Invocation Build(Expression<Func<Task>> expression)
        {
            return Build((MethodCallExpression)expression.Body);
        }

        public static Invocation Build<T>(Argument @object, Expression<Func<T, Task>> expression)
        {
            return Build(@object, (MethodCallExpression)expression.Body);
        }

        public static Invocation Build<TResult>(Expression<Func<TResult>> expression)
        {
            return Build((MethodCallExpression)expression.Body);
        }

        public static Invocation Build<T, TResult>(Argument @object, Expression<Func<T, TResult>> expression)
        {
            return Build(@object, (MethodCallExpression)expression.Body);
        }

        public static Invocation Build<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return Build((MethodCallExpression)expression.Body);
        }

        public static Invocation Build<T, TResult>(Argument @object, Expression<Func<T, Task<TResult>>> expression)
        {
            return Build(@object, (MethodCallExpression)expression.Body);
        }

        private static Argument GetObjectArgument(MethodCallExpression expression)
        {
            if (expression.Object == null)
            {
                return Argument.Static;
            }
            return Argument.Value(ExpressionHelper.Evaluate(expression.Object));
        }

        private static Method GetMethod(MethodCallExpression expression)
        {
            return new Method(
                expression.Method.DeclaringType,
                expression.Method.Name,
                expression.Method.GetParameters()
                    .Select(parameterInfo => parameterInfo.ParameterType)
                    .ToList());
        }

        private static List<Argument> GetArguments(MethodCallExpression expression)
        {
            return expression.Arguments
                .Select(ExpressionHelper.Evaluate)
                .Select(Argument.Value)
                .ToList();
        }
    }
}
