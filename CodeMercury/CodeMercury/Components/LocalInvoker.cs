using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class LocalInvoker : IInvoker
    {
        public async Task<object> InvokeAsync(MethodInfo method, object[] arguments)
        {
            object result;
            try
            {
                result = method.Invoke(null, arguments);
            }
            catch (TargetInvocationException exception)
            {
                throw new InvocationException(exception.InnerException);
            }
            if (method.ReturnType.IsSubclassOf(typeof(Task)))
            {
                try
                {
                    return await ((dynamic)result).ConfigureAwait(false);
                }
                catch (TargetInvocationException exception)
                {
                    throw new InvocationException(exception.InnerException);
                }
            }
            else if (method.ReturnType.Equals(typeof(Task)))
            {
                try
                {
                    await ((dynamic)result).ConfigureAwait(false);
                }
                catch (TargetInvocationException exception)
                {
                    throw new InvocationException(exception.InnerException);
                }
                return null;
            }
            else
            {
                return result;
            }
        }
    }
}
