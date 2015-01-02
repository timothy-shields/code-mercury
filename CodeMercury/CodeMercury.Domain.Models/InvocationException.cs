using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class InvocationException : Exception
    {
        public string Content { get; private set; }

        public InvocationException(string content)
            : base("An exception occurred during an invocation. See the Content for details.", null)
        {
            this.Content = content;
        }

        public InvocationException(Exception inner)
            : base("An exception occurred during an invocation. See the InnerException for details.", inner)
        {
        }

        public InvocationException(string content, Exception inner)
            : base("An exception occurred during an invocation. See the Content and InnerException for details.", inner)
        {
            this.Content = content;
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
