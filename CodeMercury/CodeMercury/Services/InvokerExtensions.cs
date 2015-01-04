using CodeMercury.Components;
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
        public static async Task<TResult> InvokeAsync<TResult>(this IInvoker invoker, Expression<Func<Task<TResult>>> expression, IArgumentMaterializer argumentMaterializer = default(IArgumentMaterializer))
        {
            if (argumentMaterializer == null)
            {
                argumentMaterializer = ArgumentMaterializer.Default;
            }
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            var result = await (Task<TResult>)argumentMaterializer.Materialize(invocation.Method.ReturnType, resultArgument);
            return result;
        }

        public static async Task InvokeAsync(this IInvoker invoker, Expression<Func<Task>> expression, IArgumentMaterializer argumentMaterializer = default(IArgumentMaterializer))
        {
            if (argumentMaterializer == null)
            {
                argumentMaterializer = ArgumentMaterializer.Default;
            }
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            await (Task)argumentMaterializer.Materialize(invocation.Method.ReturnType, resultArgument);
        }

        public static async Task<TResult> InvokeAsync<TResult>(this IInvoker invoker, Expression<Func<TResult>> expression, IArgumentMaterializer argumentMaterializer = default(IArgumentMaterializer))
        {
            if (argumentMaterializer == null)
            {
                argumentMaterializer = ArgumentMaterializer.Default;
            }
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            var result = (TResult)argumentMaterializer.Materialize(invocation.Method.ReturnType, resultArgument);
            return result;
        }

        public static async Task InvokeAsync(this IInvoker invoker, Expression<Action> expression, IArgumentMaterializer argumentMaterializer = default(IArgumentMaterializer))
        {
            if (argumentMaterializer == null)
            {
                argumentMaterializer = ArgumentMaterializer.Default;
            }
            var invocation = InvocationBuilder.Build(expression);
            var resultArgument = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            var result = argumentMaterializer.Materialize(invocation.Method.ReturnType, resultArgument);
        }
    }
}
