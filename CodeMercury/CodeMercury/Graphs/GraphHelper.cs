using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury.Graphs
{
    public static class GraphHelper
    {
        public static Dictionary<BoolString, List<BoolString>> HammingGraph(int n, int d)
        {
            var dict = EnumerableEx
                .Generate(BoolString.First(), s => (s.BoundedCount <= n), s => s.Next(), s => s)
                .ToDictionary(s => s, s => new List<BoolString>());
            foreach (var u in dict.Keys)
            {
                foreach (var v in dict.Keys)
                {
                    var dist = BoolString.HammingDistance(u, v);
                    if (0 < dist && dist <= d)
                        dict[u].Add(v);
                }
            }
            return dict;
        }
    }
}
