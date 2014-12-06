using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    // TODO This really needs to be defined in terms of a MethodCallExpression.
    public interface IInvoker
    {
        Task<object> InvokeAsync(MethodInfo method, object[] arguments);
    }
}
