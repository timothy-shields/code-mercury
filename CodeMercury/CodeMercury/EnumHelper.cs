using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury
{
    public static class EnumHelper
    {
        public static T Parse<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static T Parse<T>(string value, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static IEnumerable<string> Names<T>()
        {
            return Enum.GetNames(typeof(T));
        }

        public static IEnumerable<T> Values<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
