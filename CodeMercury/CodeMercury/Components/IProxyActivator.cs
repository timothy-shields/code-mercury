using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public interface IProxyActivator
    {
        IProxy Create(Type proxyType, IInvoker proxyInvoker);
    }
}
