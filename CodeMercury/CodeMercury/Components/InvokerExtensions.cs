using CodeMercury.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public static class InvokerExtensions
    {
        public static async Task<TResult> InvokeAsync<TResult>(this IInvoker callScheduler, MethodCallExpression expression)
        {
            var result = await callScheduler.InvokeAsync(expression.Method, expression.Arguments.Select(ExpressionHelper.Evaluate).ToArray()).ConfigureAwait(false);
            return (TResult)result;
        }

        public static Task<TResult> InvokeAsync<TResult>(this IInvoker callScheduler, Expression<Func<TResult>> expression)
        {
            return callScheduler.InvokeAsync<TResult>((MethodCallExpression)expression.Body);
        }

        public static Task<TResult> InvokeAsync<TResult>(this IInvoker callScheduler, Expression<Func<Task<TResult>>> expression)
        {
            return callScheduler.InvokeAsync<TResult>((MethodCallExpression)expression.Body);
        }

        public static Task<TResult> InvokeAsync<T, TResult>(this IInvoker callScheduler, Expression<Func<T, Task<TResult>>> expression)
        {
            throw new NotImplementedException();
        }

        public static Task InvokeAsync<T>(this IInvoker callScheduler, Expression<Func<T, Task>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
