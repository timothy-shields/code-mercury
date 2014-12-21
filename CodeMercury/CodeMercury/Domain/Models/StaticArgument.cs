using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents the absence of an Object in an <see cref="Invocation"/> for a static method.
    /// </summary>
    public class StaticArgument : Argument
    {
        public override string ToString()
        {
            return "Static";
        }
    }
}
