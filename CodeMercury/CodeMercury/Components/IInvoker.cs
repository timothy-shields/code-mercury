using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public interface IInvoker
    {
        Task<object> InvokeAsync(MethodInfo method, object[] arguments);
    }
}
