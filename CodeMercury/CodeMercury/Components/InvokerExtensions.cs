﻿using CodeMercury.Domain.Models;
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
        public static async Task<TResult> InvokeAsync<TResult>(this IInvoker invoker, Expression<Func<Task<TResult>>> expression)
        {
            var resultArgument = await invoker.InvokeAsync((MethodCallExpression)expression.Body).ConfigureAwait(false);
            var result = resultArgument.CastTo<ValueArgument>().Value.CastTo<TResult>();
            return result;
        }

        public static async Task InvokeAsync(this IInvoker invoker, Expression<Func<Task>> expression)
        {
            var resultArgument = await invoker.InvokeAsync((MethodCallExpression)expression.Body).ConfigureAwait(false);
            if (!(resultArgument is VoidArgument))
            {
                throw new CodeMercuryBugException();
            }
        }

        public static async Task<Argument> InvokeAsync(this IInvoker invoker, MethodCallExpression expression)
        {
            var @object = GetObjectArgument(expression);
            var method = GetMethod(expression);
            var arguments = GetArguments(expression);
            var invocation = new Invocation(@object, method, arguments);
            var result = await invoker.InvokeAsync(invocation).ConfigureAwait(false);
            return result;
        }

        private static Argument GetObjectArgument(MethodCallExpression expression)
        {
            if (expression.Object == null)
            {
                return null;
            }
            return new ValueArgument(expression.Object);
        }

        private static Method GetMethod(MethodCallExpression expression)
        {
            return new Method(
                expression.Method.DeclaringType,
                expression.Method.Name,
                expression.Method.GetParameters().Select(parameter => new Parameter(parameter)));
        }

        private static IEnumerable<ValueArgument> GetArguments(MethodCallExpression expression)
        {
            return expression.Arguments
                .Select(argument => new ValueArgument(ExpressionHelper.Evaluate(argument)));
        }
    }
}