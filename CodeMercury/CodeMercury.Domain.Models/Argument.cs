using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents an input or output of an invocation.
    /// Is similar to the <see cref="Expression"/> type hierarchy.
    /// </summary>
    public abstract class Argument
    {
        private static readonly CanceledArgument canceled = new CanceledArgument();
        public static Argument Canceled
        {
            get { return canceled; }
        }

        public static Argument Exception(Exception exception)
        {
            if (exception is OperationCanceledException)
            {
                return Canceled;
            }
            return new ExceptionArgument(exception);
        }

        public static Argument Proxy(Guid serviceId)
        {
            return new ProxyArgument(serviceId);
        }

        public static Argument Service(Guid serviceId)
        {
            return new ServiceArgument(serviceId);
        }

        private static readonly StaticArgument @static = new StaticArgument();
        public static Argument Static
        {
            get { return @static; }
        }

        public static Argument Task(Argument result)
        {
            return new TaskArgument(result);
        }

        public static Argument Value(object value)
        {
            return new ValueArgument(value);
        }

        private static readonly VoidArgument @void = new VoidArgument();
        public static Argument Void
        {
            get { return @void; }
        }
    }
}
