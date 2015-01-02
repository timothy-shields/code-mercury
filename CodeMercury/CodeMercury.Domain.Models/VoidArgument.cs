using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents the result of a void- or Task-returning method.
    /// </summary>
    public class VoidArgument : Argument
    {
        internal VoidArgument()
        {
        }

        public override string ToString()
        {
            return "Void";
        }
    }
}
