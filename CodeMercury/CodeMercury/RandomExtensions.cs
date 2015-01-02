using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury
{
    public static class RandomExtensions
    {
        public static int NextInclusive(this Random random, int i)
        {
            return random.Next(i + 1);
        }

        public static bool SampleBernoulli(this Random random, double p)
        {
            return random.NextDouble() < p;
        }

        /// <summary>
        /// More efficient version of SampleBernoulli(1.0 / n).
        /// </summary>
        public static bool SampleBernoulli(this Random random, int n)
        {
            return random.Next(n) == 0;
        }

        public static T RandomItem<T>(this Random random, IList<T> source)
        {
            return source[random.Next(source.Count)];
        }

        public static T RandomItem<T>(this Random random, IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    throw new InvalidOperationException("The source sequence is empty.");
                var item = e.Current;
                var n = 2;
                while (e.MoveNext())
                {
                    if (random.SampleBernoulli(n))
                        item = e.Current;
                    n++;
                }
                return item;
            }
        }

        public static List<T> RandomItems<T>(this Random random, IList<T> source, int count)
        {
            var result = new List<T>(Math.Min(count, source.Count));
            for (var i = 0; i < count; i++)
            {
                var r = random.NextInclusive(i);
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
                var r = random.NextInclusive(i);
                if (r < count)
                {
                    result[r] = source[i];
                }
            }
            return result;
        }

        public static List<T> RandomItems<T>(this Random random, IEnumerable<T> source, int count)
        {
            var result = new List<T>(count);
            using (var e = source.GetEnumerator())
            {
                var i = 0;
                while (e.MoveNext() && i < count)
                {
                    var r = random.NextInclusive(i);
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
                    var r = random.NextInclusive(i);
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
