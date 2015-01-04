using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public sealed class CapturedException<TException> : CapturedException
        where TException : Exception
    {
        internal CapturedException(IReadOnlyCollection<string> contents)
            : base(typeof(TException), contents)
        {
        }
    }

    public class CapturedException : Exception
    {
        public Type ExceptionType { get; private set; }
        public IReadOnlyCollection<string> Contents { get; private set; }

        protected internal CapturedException(Type exceptionType, IReadOnlyCollection<string> contents)
            : base(string.Format("An exception of type {0} was captured and rethrown.", exceptionType.Name))
        {
            this.ExceptionType = exceptionType;
            this.Contents = contents;
        }

        public string ToDebugString()
        {
            var sb = new StringBuilder();
            int i = 0;
            sb.AppendFormat("[{0}]", i++);
            sb.AppendLine();
            sb.AppendLine(base.ToString());
            foreach (var content in Contents)
            {
                sb.AppendFormat("[{0}] ", i++);
                sb.AppendLine();
                sb.AppendLine(content);
            }
            return sb.ToString();
        }

        public static CapturedException Create(Type exceptionType, IReadOnlyCollection<string> contents)
        {
            // We don't want to ever end up with a CapturedException<CapturedException<TException>>.
            // So, if we ever try to create one, instead create a CapturedException<TException>.
            if (exceptionType.IsSubclassOf(typeof(CapturedException)))
            {
                return Create(exceptionType.GetGenericArguments().Single(), contents);
            }
            var capturedExceptionType = typeof(CapturedException<>).MakeGenericType(exceptionType);
            var capturedException = (CapturedException)Activator.CreateInstance(
                capturedExceptionType,
                BindingFlags.NonPublic | BindingFlags.Instance, default(Binder),
                new object[] { contents },
                default(CultureInfo));
            return capturedException;
        }

        public static CapturedException Create(Exception exception)
        {
            // We don't want to ever end up with a CapturedException<CapturedException<TException>>.
            // So, if we ever try to create one, instead create a CapturedException<TException> and push
            // the content of the nested CapturedException onto the stack of contents.
            if (exception is CapturedException)
            {
                var capturedException = (CapturedException)exception;
                var contents = new List<string>();
                contents.Add(capturedException.ToString());
                contents.AddRange(capturedException.Contents);
                return Create(capturedException.ExceptionType, contents);
            }
            return Create(exception.GetType(), new List<string> { exception.ToString() });
        }
    }
}
