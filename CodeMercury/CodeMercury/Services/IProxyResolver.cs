using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Services
{
    /// <summary>
    /// Resolves proxy instances.
    /// </summary>
    public interface IProxyResolver
    {
        /// <summary>
        /// Resolves a proxy instance.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The proxy instance.</returns>
        object Resolve(Guid serviceId, Type serviceType);
    }
}
