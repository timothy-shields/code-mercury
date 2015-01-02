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
    public static class InvokerExtensions
    {
        public static async Task<TResult> InvokeAsync<TResult>(this IInvoker invoker, Expression<Func<Task<TResult>>> expression)
        {
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            var result = resultArgument.CastTo<TaskArgument>().Result.CastTo<ValueArgument>().Value.CastTo<TResult>();
            return result;
        }

        public static async Task InvokeAsync(this IInvoker invoker, Expression<Func<Task>> expression)
        {
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            var result = resultArgument.CastTo<TaskArgument>().Result.CastTo<VoidArgument>();
        }

        public static async Task<TResult> InvokeAsync<TResult>(this IInvoker invoker, Expression<Func<TResult>> expression)
        {
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            var result = resultArgument.CastTo<ValueArgument>().Value.CastTo<TResult>();
            return result;
        }

        public static async Task InvokeAsync(this IInvoker invoker, Expression<Action> expression)
        {
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            var result = resultArgument.CastTo<VoidArgument>();
        }

        public static async Task<Argument> InvokeAsync(this IInvoker invoker, MethodCallExpression expression)
        {
            var invocation = InvocationBuilder.Build(expression);
            var result = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            return result;
        }
    }
}
