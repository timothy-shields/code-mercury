using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class ValueArgument : Argument
    {
        public object Value { get; private set; }

        public ValueArgument(object value)
        {
            this.Value = value;
        }
    }
}
