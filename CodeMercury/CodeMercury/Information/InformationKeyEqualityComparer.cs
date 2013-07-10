using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Information
{
    public class InformationKeyEqualityComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            return Enumerable.SequenceEqual(x, y);
        }

        public int GetHashCode(byte[] obj)
        {
            return BitConverter.ToInt32(obj, 0);
        }
    }
}
