using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury
{
    public static class StringHelper
    {
        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
