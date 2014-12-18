using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    /// <summary>
    /// Resolves local service instances.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>
        /// Resolves a local service instance by ID.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <returns>The service instance.</returns>
        object Resolve(Guid serviceId);
    }
}
