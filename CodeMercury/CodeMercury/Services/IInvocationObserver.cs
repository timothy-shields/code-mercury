using CodeMercury.Domain.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMercury.Services
{
    public interface IInvocationObserver
    {
        void OnResult(Guid key, Argument result);
        void OnCancellation(Guid key);
        void OnException(Guid key, InvocationException exception);
    }
}
