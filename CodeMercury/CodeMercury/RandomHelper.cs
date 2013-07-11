using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury
{
    public static class RandomHelper
    {
        private static Random random = new Random();

        public static int Next(int n)
        {
            return Functional.Lock(random, () => random.Next(n));
        }

        public static int NextInclusive(int i)
        {
            return Next(i + 1);
        }

        public static double NextDouble()
        {
            return Functional.Lock(random, () => random.NextDouble());
        }

        public static bool SampleBernoulli(double p)
        {
            return NextDouble() < p;
        }

        /// <summary>
        /// More efficient version of SampleBernoulli(1.0 / n).
        /// </summary>
        public static bool SampleBernoulli(int n)
        {
            return Next(n) == 0;
        }

        public static T RandomItem<T>(IList<T> source)
        {
            return source[Next(source.Count)];
        }

        public static T RandomItem<T>(IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    throw new InvalidOperationException("The source sequence is empty.");
                var item = e.Current;
                var n = 2;
                while (e.MoveNext())
                {
                    if (SampleBernoulli(n))
                        item = e.Current;
                    n++;
                }
                return item;
            }
        }

        public static List<T> RandomItems<T>(IList<T> source, int count)
        {
            var result = new List<T>(Math.Min(count, source.Count));
            for (var i = 0; i < count; i++)
            {
                var r = NextInclusive(i);
                if (r < i)
                {
                    result.Add(result[r]);
                    result[r] = source[i];
                }
                else
                {
                    result.Add(source[i]);
                }
            }
            for (var i = count; i < source.Count; i++)
            {
                var r = NextInclusive(i);
                if (r < count)
                {
                    result[r] = source[i];
                }
            }
            return result;
        }

        public static List<T> RandomItems<T>(IEnumerable<T> source, int count)
        {
            var result = new List<T>(count);
            using (var e = source.GetEnumerator())
            {
                var i = 0;
                while (e.MoveNext() && i < count)
                {
                    var r = NextInclusive(i);
                    if (r < i)
                    {
                        result.Add(result[r]);
                        result[r] = e.Current;
                    }
                    else
                    {
                        result.Add(e.Current);
                    }
                    i++;
                }
                while (e.MoveNext())
                {
                    var r = NextInclusive(i);
                    if (r < count)
                    {
                        result[r] = e.Current;
                    }
                    i++;
                }
            }
            return result;
        }
    }
}
