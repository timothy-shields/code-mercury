using CodeMercury.Domain.Models;
using CodeMercury.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// Invokes <see cref="Invocation"/> instances on the local machine.
    /// </summary>
    public class LocalInvoker : IInvoker
    {
        private readonly IServiceResolver serviceResolver;
        private readonly IProxyResolver proxyResolver;

        /// <summary>
        /// Constructs a <see cref="LocalInvoker"/>.
        /// </summary>
        /// <param name="serviceResolver">The <see cref="IServiceResolver"/> to use when resolving service instances from <see cref="ServiceArgument"/> arguments.</param>
        /// <param name="proxyResolver">The <see cref="IProxyResolver"/> to use when resolving proxy instances from <see cref="ProxyArgument"/> arguments.</param>
        public LocalInvoker(IServiceResolver serviceResolver, IProxyResolver proxyResolver)
        {
            this.serviceResolver = serviceResolver;
            this.proxyResolver = proxyResolver;
        }

        public async Task<Argument> InvokeAsync(Invocation invocation)
        {
            var @object = ResolveObject(invocation.Object);
            var method = ResolveMethod(invocation.Method, @object);
            var arguments = method.ParameterTypes.Zip(invocation.Arguments,
                (parameterType, argument) => ResolveArgument(parameterType, argument)).ToArray();
            object result;
            try
            {
                result = method.MethodInfo.Invoke(@object, arguments);
            }
            catch (TargetInvocationException exception)
            {
                return Argument.Exception(exception.InnerException);
            }
            return await CreateResultAsync(method.ReturnType, result);
        }

        private object ResolveObject(Argument @object)
        {
            if (@object is ServiceArgument)
            {
                return serviceResolver.Resolve(@object.CastTo<ServiceArgument>().ServiceId);
            }
            if (@object is StaticArgument)
            {
                return null;
            }
            throw new CodeMercuryBugException();
        }

        private Method ResolveMethod(Method method, object @object)
        {
            if (@object == null || @object.GetType() == method.DeclaringType)
            {
                return method;
            }
            return method.WithDeclaringType(@object.GetType());
        }

        private object ResolveArgument(Type parameterType, Argument argument)
        {
            if (argument is ProxyArgument)
            {
                return proxyResolver.Resolve(argument.CastTo<ProxyArgument>().ServiceId, parameterType);
            }
            if (argument is ValueArgument)
            {
                return argument.CastTo<ValueArgument>().Value;
            }
            throw new CodeMercuryBugException();
        }

        private static async Task<Argument> CreateResultAsync(Type type, object result)
        {
            if (type.IsSubclassOf(typeof(Task)))
            {
                var resultType = type.GetGenericArguments().Single();
                object value;
                try
                {
                    value = await ((dynamic)result).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    return Argument.Task(Argument.Exception(exception));
                }
                var taskResult = await CreateResultAsync(resultType, value).ConfigureAwait(false);
                return Argument.Task(taskResult);
            }
            if (type.Equals(typeof(Task)))
            {
                try
                {
                    await result.CastTo<Task>().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    return Argument.Task(Argument.Exception(exception));
                }
                return Argument.Task(Argument.Void);
            }
            if (type.Equals(typeof(void)))
            {
                return Argument.Void;
            }
            return Argument.Value(result);
        }
    }
}
