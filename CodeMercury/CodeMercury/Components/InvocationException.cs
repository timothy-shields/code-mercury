using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class InvocationException : Exception
    {
        public string Content { get; private set; }

        public InvocationException(string content)
            : this(content, null)
        {
        }

        public InvocationException(Exception inner)
            : this("An exception occurred during an invocation. See the InnerException for details.", inner)
        {
        }

        public InvocationException(string content, Exception inner)
            : base("An exception occurred during an invocation. See the Content for details.", inner)
        {
        }

        public override string ToString()
        {
            if (Content != null)
            {
                return string.Join(Environment.NewLine, base.ToString(), "Content:", Content);
            }
            return base.ToString();
        }
    }
}
