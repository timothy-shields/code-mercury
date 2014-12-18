using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents the service end of a service-proxy link.
    /// The sender of a <see cref="ServiceArgument"/> is on a different machine than the service.
    /// The recipient of a <see cref="ServiceArgument"/> is on the same machine as the service.
    /// </summary>
    public class ServiceArgument : Argument
    {
        public Guid ServiceId { get; private set; }

        public ServiceArgument(Guid serviceId)
        {
            this.ServiceId = serviceId;
        }
    }
}
