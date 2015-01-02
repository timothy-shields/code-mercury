using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents an invocation of a method.
    /// </summary>
    public class Invocation
    {
        /// <summary>
        /// The static type or instance the method is being called on.
        /// </summary>
        public Argument Object { get; private set; }
        
        /// <summary>
        /// The method being invoked.
        /// </summary>
        public Method Method { get; private set; }

        /// <summary>
        /// The arguments to pass to the method when invoking it.
        /// </summary>
        public IReadOnlyCollection<Argument> Arguments { get; private set; }

        public Invocation(Argument @object, Method method, IReadOnlyCollection<Argument> arguments)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            this.Object = @object;
            this.Method = method;
            this.Arguments = arguments;
        }

        public override string ToString()
        {
            return string.Format("Invocation({0}, {1}.{2}({3}))", Object, Method.Name, string.Join(", ", Arguments));
        }
    }
}
