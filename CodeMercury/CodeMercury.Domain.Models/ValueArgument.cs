using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents a value.
    /// </summary>
    public class ValueArgument : Argument
    {
        public new object Value { get; private set; }

        internal ValueArgument(object value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("Value({0})", Value);
        }
    }
}
