﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public interface IProxyContainer
    {
        void Register(Guid id, Type proxyType);
    }
}
