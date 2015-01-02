using CodeMercury.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// Instantiates proxies.
    /// </summary>
    public class ProxyActivator : IProxyActivator
    {
        private readonly Dictionary<Type, Func<IInvoker, object>> activators;

        public ProxyActivator(IDictionary<Type, Func<IInvoker, object>> activators)
        {
            this.activators = new Dictionary<Type, Func<IInvoker, object>>(activators);
        }

        /// <summary>
        /// Instantiates a proxy.
        /// </summary>
        /// <param name="serviceType">The type of the remote service.</param>
        /// <param name="proxyInvoker">The invoker to use in the proxy.</param>
        /// <returns>The proxy instance.</returns>
        public object Create(Type serviceType, IInvoker proxyInvoker)
        {
            if (!serviceType.IsInterface)
            {
                throw new CodeMercuryBugException();
            }
            var activator = activators[serviceType];
            var proxy = activator(proxyInvoker);
            return proxy;
        }
    }
}
