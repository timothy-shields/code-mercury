using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class CanceledArgument : Argument
    {
        internal CanceledArgument()
        {
        }

        public override string ToString()
        {
            return "Canceled";
        }
    }
}
