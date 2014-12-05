using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Information
{
    public static class InformationHelper
    {
        public static byte[] GetInformationKey(byte[] information)
        {
            return HashHelper.ComputeHash(information);
        }
    }
}
