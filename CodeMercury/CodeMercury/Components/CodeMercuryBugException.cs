using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class CodeMercuryBugException : Exception
    {
        public CodeMercuryBugException()
            : this(null)
        {
        }

        public CodeMercuryBugException(Exception innerException)
            : base("This should never happen. There is a bug in CodeMercury.", innerException)
        {
        }
    }
}
