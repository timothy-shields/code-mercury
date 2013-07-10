using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Information
{
    public class InformationSet : Dictionary<byte[], byte[]>
    {
        public InformationSet()
            : base(new InformationKeyEqualityComparer())
        {
        }
    }
}
