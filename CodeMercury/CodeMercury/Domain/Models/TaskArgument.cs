using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class TaskArgument : Argument
    {
        public Argument Result { get; private set; }

        public TaskArgument(Argument result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            this.Result = result;
        }
    }
}
