using CodeMercury.Domain.Models;
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
            var method = invocation.Method.ToMethodInfo();
            var @object = ResolveObject(invocation.Object);
            var arguments = invocation.Arguments.Select(ResolveArgument).ToArray();
            var result = Invoke(@object, method, arguments);
            return await CreateResultAsync(method, result);
        }

        private object ResolveObject(Argument @object)
        {
            if (@object == null)
            {
                return null;
            }
            if (@object is ServiceArgument)
            {
                return serviceResolver.Resolve(@object.CastTo<ServiceArgument>().ServiceId);
            }
            throw new CodeMercuryBugException();
        }

        private static object Invoke(object @object, MethodInfo method, object[] arguments)
        {
            try
            {
                return method.Invoke(@object, arguments);
            }
            catch (TargetInvocationException exception)
            {
                throw new InvocationException(exception.InnerException);
            }
        }

        private object ResolveArgument(Argument argument)
        {
            if (argument is ProxyArgument)
            {
                return proxyResolver.Resolve(argument.CastTo<ProxyArgument>().ServiceId);
            }
            if (argument is ValueArgument)
            {
                return argument.CastTo<ValueArgument>().Value;
            }
            throw new CodeMercuryBugException();
        }

        private static async Task<Argument> CreateResultAsync(MethodInfo method, object result)
        {
            if (method.ReturnType.IsSubclassOf(typeof(Task)))
            {
                try
                {
                    object value = await ((dynamic)result).ConfigureAwait(false);
                    return new ValueArgument(value);
                }
                catch (TargetInvocationException exception)
                {
                    throw new InvocationException(exception.InnerException);
                }
            }
            if (method.ReturnType.Equals(typeof(Task)))
            {
                try
                {
                    await ((dynamic)result).ConfigureAwait(false);
                    return new VoidArgument();
                }
                catch (TargetInvocationException exception)
                {
                    throw new InvocationException(exception.InnerException);
                }
            }
            return new ValueArgument(result);
        }
    }
}
