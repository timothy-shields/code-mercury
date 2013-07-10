using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury
{
    public static class HashHelper
    {
        public static byte[] ComputeHash(byte[] data)
        {
            using (var sha1 = SHA1.Create())
                return sha1.ComputeHash(data);
        }
    }
}
