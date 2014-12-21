using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Services
{
    /// <summary>
    /// Instantiates proxies.
    /// </summary>
    public interface IProxyActivator
    {
        /// <summary>
        /// Instantiates a proxy.
        /// </summary>
        /// <param name="serviceType">The type of the remote service.</param>
        /// <param name="proxyInvoker">The invoker to use in the proxy.</param>
        /// <returns>The proxy instance.</returns>
        IProxy Create(Type serviceType, IInvoker proxyInvoker);
    }
}
