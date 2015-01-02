using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class CapturedException<TException> : Exception
        where TException : Exception
    {
        public string Content { get; private set; }

        internal CapturedException(string content)
            : base(string.Format("An exception of the type {0} was captured and rethrown.", typeof(TException).Name))
        {
            this.Content = content;
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine + "Captured exception:" + Environment.NewLine + Content;
        }
    }

    public static class CapturedException
    {
        public static Exception Create(Type exceptionType, string content)
        {
            // We don't want to ever end up with a CapturedException<CapturedException<TException>>.
            // So, if we ever try to create one, instead create a CapturedException<TException>.
            if (exceptionType.GetGenericTypeDefinition().Equals(typeof(CapturedException<>)))
            {
                return Create(exceptionType.GetGenericArguments().Single(), content);
            }
            return (Exception)Activator.CreateInstance(typeof(CapturedException<>).MakeGenericType(exceptionType), content);
        }

        public static Exception Create<TException>(string content)
        {
            return Create(typeof(TException), content);
        }

        public static Exception Create(Exception exception)
        {
            return Create(exception.GetType(), exception.ToString());
        }
    }
}
