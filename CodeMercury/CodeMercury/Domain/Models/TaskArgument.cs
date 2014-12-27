using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents an argument that has a type of <see cref="Task"/> or <see cref="Task&lt;T&gt;"/>.
    /// </summary>
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

        public override string ToString()
        {
            return string.Format("Task({0})", Result);
        }
    }
}
