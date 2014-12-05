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
    public class HttpInvoker : IInvoker, IDisposable
    {
        private HttpClient client;
        private IDisposable completionsSubscription;

        private ConcurrentDictionary<Guid, CallContext> contexts = new ConcurrentDictionary<Guid, CallContext>();

        private class CallContext : IDisposable
        {
            public MethodInfo Method { get; private set; }
            public object[] Arguments { get; private set; }
            public Call Call { get; private set; }
            public Guid CallId { get { return Call.CallId; } }
            private TaskCompletionSource<object> taskCompletionSource;
            private CancellationTokenRegistration cancellationTokenRegistration;

            public Task Completion { get; private set; }

            public CallContext(MethodInfo method, object[] arguments)
            {
                Method = method;
                Arguments = arguments;
                Call = new Call
                {
                    CallId = Guid.NewGuid(),
                    Function = new Function
                    {
                        DeclaringType = method.DeclaringType,
                        Name = method.Name,
                        Parameters = method.GetParameters()
                            .Select(parameter => new Parameter
                            {
                                ParameterType = parameter.ParameterType,
                                Name = parameter.Name
                            })
                            .ToList()
                    },
                    Arguments = Enumerable.Zip(method.GetParameters(), arguments, ConvertArgument).ToList()
                };
                taskCompletionSource = new TaskCompletionSource<object>();
                Completion = taskCompletionSource.Task;
            }

            private JToken ConvertArgument(ParameterInfo parameter, object argument)
            {
                if (parameter.ParameterType.Equals(typeof(CancellationToken)))
                {
                    var cancellationToken = (CancellationToken)argument;
                    cancellationTokenRegistration = cancellationToken.Register(OnCanceling, false);
                    return null;
                }
                return JToken.FromObject(argument);
            }

            private void OnCanceling()
            {
                taskCompletionSource.TrySetCanceled();
            }

            public void OnCompleted(CallCompletion completion)
            {
                taskCompletionSource.TrySetResult(null);
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        public HttpInvoker(HttpClient client, IObservable<CallCompletion> completions)
        {
            this.client = client;
            completionsSubscription = completions.Subscribe(OnCompleted);
        }

        public async Task<object> InvokeAsync(MethodInfo method, object[] arguments)
        {
            var context = new CallContext(method, arguments);
            if (!contexts.TryAdd(context.CallId, context))
            {
                //TODO: need more elegant handling of this case
                throw new Exception("This shouldn't ever happen.");
            }
            var putCallResponse = await client.PostAsJsonAsync("calls", context.Call).ConfigureAwait(false);
            var receipt = await putCallResponse.Content.ReadAsAsync<CallReceipt>().ConfigureAwait(false);
            await context.Completion.ConfigureAwait(false);
            var getCallResponse = await client.GetAsync(receipt.CallUri).ConfigureAwait(false);
            var call = await getCallResponse.Content.ReadAsAsync<Call>();
            switch (call.Status)
            {
                case CallStatus.RanToCompletion:
                    return call.Result.ToObject(context.Method.ReturnType);
                case CallStatus.Canceled:
                    throw new OperationCanceledException();
                case CallStatus.Faulted:
                    throw new InvocationException(call.Exception.Content);
                default:
                    throw new Exception("This shouldn't ever happen.");
            }
        }

        private void OnCompleted(CallCompletion completion)
        {
            CallContext context;
            if (!contexts.TryRemove(completion.CallId, out context))
            {
                //TODO: need more elegant handling of this case
                throw new Exception("This shouldn't ever happen.");
            }
            context.OnCompleted(completion);
        }

        public void Dispose()
        {
            completionsSubscription.Dispose();
        }
    }
}
