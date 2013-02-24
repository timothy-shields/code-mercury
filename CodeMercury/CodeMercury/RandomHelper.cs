using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury
{
    public static class RandomHelper
    {
        private static Random random = new Random();

        public static T RandomItem<T>(IList<T> list)
        {
            return list[Functional.Lock(random, () => random.Next(list.Count))];
        }
    }
}
