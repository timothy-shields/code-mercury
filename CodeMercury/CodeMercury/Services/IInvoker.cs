using CodeMercury.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Services
{
    /// <summary>
    /// Invokes <see cref="Invocation"/> instances.
    /// </summary>
    public interface IInvoker
    {
        Task<Argument> InvokeAsync(Invocation invocation);
    }
}
