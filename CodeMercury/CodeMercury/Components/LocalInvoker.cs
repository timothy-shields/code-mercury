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
        private readonly IProxyResolver proxyResolver;

        /// <summary>
        /// Constructs a <see cref="LocalInvoker"/>.
        /// </summary>
        /// <param name="proxyResolver">The <see cref="IProxyResolver"/> to use when resolving <see cref="IProxy"/> instances from <see cref="ProxyArgument"/> arguments.</param>
        public LocalInvoker(IProxyResolver proxyResolver)
        {
            if (proxyResolver == null)
            {
                throw new ArgumentNullException("proxyResolver");
            }

            this.proxyResolver = proxyResolver;
        }

        public async Task<Argument> InvokeAsync(Invocation invocation)
        {
            var method = invocation.Method.ToMethodInfo();
            var instance = GetInstanceArgument(invocation.Object);
            var arguments = invocation.Arguments.Select(ResolveArgument).ToArray();
            var result = Invoke(method, instance, arguments);
            return await CreateResultArgumentAsync(method, result);
        }

        private object GetInstanceArgument(Argument instance)
        {
            if (instance == null)
            {
                return null;
            }
            return ResolveArgument(instance);
        }

        private static object Invoke(MethodInfo method, object instance, object[] arguments)
        {
            try
            {
                return method.Invoke(instance, arguments);
            }
            catch (TargetInvocationException exception)
            {
                throw new InvocationException(exception.InnerException);
            }
        }

        private object ResolveArgument(Argument argument)
        {
            if (argument is ValueArgument)
            {
                return ResolveValueArgument((ValueArgument)argument);
            }
            else if (argument is ProxyArgument)
            {
                return ResolveProxyArgument((ProxyArgument)argument);
            }
            else
            {
                throw new CodeMercuryBugException();
            }
        }

        private static object ResolveValueArgument(ValueArgument valueArgument)
        {
            return valueArgument.Value;
        }

        private object ResolveProxyArgument(ProxyArgument proxyArgument)
        {
            return proxyResolver.Resolve(proxyArgument.Id);
        }

        private static async Task<Argument> CreateResultArgumentAsync(MethodInfo method, object result)
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
            else if (method.ReturnType.Equals(typeof(Task)))
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
            else
            {
                return new ValueArgument(result);
            }
        }
    }
}
