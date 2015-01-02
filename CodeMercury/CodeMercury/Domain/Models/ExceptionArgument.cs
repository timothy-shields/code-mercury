using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class ExceptionArgument : Argument
    {
        public Exception Exception { get; private set; }

        public ExceptionArgument(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.Exception = exception;
        }
    }
}
