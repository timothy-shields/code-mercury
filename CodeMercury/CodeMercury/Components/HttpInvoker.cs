using CodeMercury.Domain.Models;
using CodeMercury.Expressions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class HttpInvoker : IInvoker, IInvocationObserver
    {
        private readonly Uri requesterUri;
        private readonly HttpClient client;
        private readonly IProxyableContainer proxyableContainer;
        private readonly ConcurrentDictionary<Guid, InvocationContext> contexts = new ConcurrentDictionary<Guid, InvocationContext>();

        private class InvocationContext : IDisposable
        {
            public Guid Key { get; private set; }
            public Invocation Invocation { get; private set; }
            private IDisposable disposable;
            private TaskCompletionSource<Argument> taskCompletionSource;

            public Task<Argument> Task
            {
                get { return taskCompletionSource.Task; }
            }

            public InvocationContext(Guid key, Invocation invocation, IDisposable disposable)
            {
                Key = key;
                Invocation = invocation;
                this.disposable = disposable;
                taskCompletionSource = new TaskCompletionSource<Argument>();
            }

            public void TrySetResult(Argument result)
            {
                taskCompletionSource.TrySetResult(result);
            }

            public void TrySetCanceled()
            {
                taskCompletionSource.TrySetCanceled();
            }

            public void TrySetException(InvocationException exception)
            {
                taskCompletionSource.TrySetException(exception);
            }

            public void Dispose()
            {
                disposable.Dispose();
            }
        }

        public HttpInvoker(Uri requesterUri, Uri serverUri, IProxyableContainer proxyableContainer)
        {
            this.requesterUri = requesterUri;
            this.client = new HttpClient { BaseAddress = serverUri };
            this.proxyableContainer = proxyableContainer;
        }

        public async Task<Argument> InvokeAsync(Invocation invocation)
        {
            using (var context = CreateContext(invocation))
            {
                var request = new WebApi.Models.InvocationRequest
                {
                    RequesterUri = requesterUri,
                    InvocationId = context.Key,
                    Object = null, //TODO Handle instance method invocations
                    Method = ConvertMethod(invocation.Method),
                    Arguments = invocation.Arguments.Select(ConvertArgument).Select(JToken.FromObject).ToList()
                };
                var response = await client.PostAsJsonAsync("invocations", request);
                response.EnsureSuccessStatusCode();
                return await context.Task;
            }
        }

        private static WebApi.Models.Method ConvertMethod(Method method)
        {
            return new WebApi.Models.Method
            {
                DeclaringType = method.DeclaringType,
                Name = method.Name,
                Parameters = method.Parameters
                    .Select(parameter => new WebApi.Models.Parameter
                    {
                        ParameterType = parameter.ParameterType,
                        Name = parameter.Name
                    })
                    .ToList()
            };
        }

        private WebApi.Models.Argument ConvertArgument(Argument argument)
        {
            if (argument is ValueArgument)
            {
                return ConvertValueArgument((ValueArgument)argument);
            }
            else
            {
                throw new CodeMercuryBugException();
            }
        }

        private WebApi.Models.Argument ConvertValueArgument(ValueArgument valueArgument)
        {
            var value = valueArgument.Value;
            if (IsProxyable(value))
            {
                var proxyId = Guid.NewGuid();
                proxyableContainer.Register(proxyId, value);
                return new WebApi.Models.ProxyArgument
                {
                    ProxyId = proxyId
                };
            }
            else
            {
                return new WebApi.Models.ValueArgument
                {
                    Value = JToken.FromObject(value)
                };
            }
        }

        private static bool IsProxyable(object value)
        {
            return value != null && value.GetType().GetInterfaces()
                .Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IProxy<>));
        }

        private InvocationContext CreateContext(Invocation invocation)
        {
            var key = Guid.NewGuid();
            var context = new InvocationContext(key, invocation, Disposable.Create(() =>
            {
                InvocationContext junk;
                contexts.TryRemove(key, out junk);
            }));
            if (!contexts.TryAdd(key, context))
            {
                //TODO: need more elegant handling of this case
                throw new CodeMercuryBugException();
            }
            return context;
        }

        private InvocationContext GetContext(Guid key)
        {
            InvocationContext context;
            if (!contexts.TryGetValue(key, out context))
            {
                throw new CodeMercuryBugException();
            }
            return context;
        }

        public void OnResult(Guid key, Argument result)
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
