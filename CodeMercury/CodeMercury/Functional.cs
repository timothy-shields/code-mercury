using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury
{
    public static class Functional
    {
        public static T CastTo<T>(this object @object)
        {
            return (T)@object;
        }
    }
}
