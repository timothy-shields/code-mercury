using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class ExceptionArgument : Argument
    {
        public new Exception Exception { get; private set; }

        internal ExceptionArgument(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.Exception = exception;
        }

        public override string ToString()
        {
            return string.Format("Exception({0})", Exception.GetType().Name);
        }
    }
}
