using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// Resolves proxy instances.
    /// </summary>
    public interface IProxyResolver
    {
        /// <summary>
        /// Resolves a proxy instance by service ID.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <returns>The proxy instance.</returns>
        IProxy Resolve(Guid serviceId);
    }
}
