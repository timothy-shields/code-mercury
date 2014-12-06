using CodeMercury.Domain.Models;
using CodeMercury.Expressions;
using CodeMercury.WebApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class HttpInvoker : IInvoker, IInvocationObserver
    {
        private HttpClient client;
        private ConcurrentDictionary<Guid, InvocationContext> contexts;

        private class InvocationContext
        {
            public MethodInfo Method { get; private set; }
            private TaskCompletionSource<object> taskCompletionSource;

            public Task<object> Task
            {
                get { return taskCompletionSource.Task; }
            }

            public InvocationContext(MethodInfo method)
            {
                Method = method;
                taskCompletionSource = new TaskCompletionSource<object>();
            }

            public void TrySetResult(JToken result)
            {
                //TODO needs to be the *effective* return type of the method, not the actual return type (Task<T> vs T)
                taskCompletionSource.TrySetResult(result.ToObject(Method.ReturnType));
            }

            public void TrySetCanceled()
            {
                taskCompletionSource.TrySetCanceled();
            }

            public void TrySetException(InvocationException exception)
            {
                taskCompletionSource.TrySetException(exception);
            }
        }

        public HttpInvoker(HttpClient client)
        {
            this.client = client;
            this.contexts = new ConcurrentDictionary<Guid, InvocationContext>();
        }

        public async Task<object> InvokeAsync(MethodInfo method, object[] arguments)
        {
            var key = Guid.NewGuid();
            var context = new InvocationContext(method);
            if (!contexts.TryAdd(key, context))
            {
                //TODO: need more elegant handling of this case
                throw new Exception("This shouldn't ever happen.");
            }
            try
            {
                var request = new CallRequest
                {
                    RequesterUri = new Uri("http://localhost:9090/"),
                    CallId = Guid.NewGuid(),
                    Function = new WebApi.Models.Function
                    {
                        DeclaringType = method.DeclaringType,
                        Name = method.Name,
                        Parameters = method.GetParameters().Select(parameter => new WebApi.Models.Parameter
                        {
                            ParameterType = parameter.ParameterType,
                            Name = parameter.Name
                        }).ToList()
                    },
                    Arguments = arguments.Select(argument => JToken.FromObject(argument)).ToList()
                };
                var response = await client.PostAsJsonAsync("calls", request);
                response.EnsureSuccessStatusCode();
                return await context.Task;
            }
            finally
            {
                contexts.TryRemove(key, out context);
            }
        }

        private InvocationContext GetContext(Guid key)
        {
            InvocationContext context;
            if (!contexts.TryGetValue(key, out context))
            {
                throw new Exception("This should never happen.");
            }
            return context;
        }

        public void OnResult(Guid key, JToken result)
        {
            var context = GetContext(key);
            context.TrySetResult(result);
        }

        public void OnCancellation(Guid key)
        {
            var context = GetContext(key);
            context.TrySetCanceled();
        }

        public void OnException(Guid key, InvocationException exception)
        {
            var context = GetContext(key);
            context.TrySetException(exception);
        }
    }
}
