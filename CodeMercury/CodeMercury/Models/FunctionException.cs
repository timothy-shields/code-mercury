using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury.Models
{
    public class FunctionException : Exception
    {
        private string _ToString;
        public FunctionException(string _ToString)
        {
            this._ToString = _ToString;
        }
        public override string ToString()
        {
            return _ToString;
        }
    }
}
