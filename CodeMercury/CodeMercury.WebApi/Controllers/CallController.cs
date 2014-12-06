using CodeMercury.WebApi.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CodeMercury.Components;
using System.Net.Http;
using System.Threading;
using System.Net;
using Nito.AspNetBackgroundTasks;

namespace CodeMercury.WebApi.Controllers
{
    public class CallController : ApiController
    {
        private readonly IInvoker invoker;
        private readonly HttpClient client;
        private readonly IInvocationObserver observer;

        public CallController(IInvoker invoker, IInvocationObserver observer)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException("invoker");
            }
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            this.invoker = invoker;
            this.client = new HttpClient();
            this.observer = observer;
        }

        [Route("calls")]
        public void PostCall(CallRequest request)
        {
            BackgroundTaskManager.Run(async () =>
            {
                var cancellationToken = BackgroundTaskManager.Shutdown;
                var method = request.Function.DeclaringType.GetMethod(
                    request.Function.Name,
                    request.Function.Parameters
                        .Select(parameter => parameter.ParameterType)
                        .ToArray());
                var arguments =
                    Enumerable.Zip(
                        request.Function.Parameters,
                        request.Arguments,
                        (parameter, argument) =>
                        {
                            if (parameter.ParameterType == typeof(CancellationToken))
                            {
                                return cancellationToken;
                            }
                            return argument.ToObject(parameter.ParameterType);
                        })
                    .ToArray();
                CallCompletion completion = null;
                object result = null;
                try
                {
                    result = await invoker.InvokeAsync(method, arguments);
                }
                catch (OperationCanceledException)
                {
                    completion = new CallCompletion
                    {
                        CallId = request.CallId,
                        Status = CallStatus.Canceled
                    };
                }
                catch (Exception exception)
                {
                    completion = new CallCompletion
                    {
                        CallId = request.CallId,
                        Status = CallStatus.Faulted,
                        Exception = new CallException
                        {
                            Content = exception.ToString()
                        }
                    };
                }
                if (completion == null)
                {
                    completion = new CallCompletion
                    {
                        CallId = request.CallId,
                        Status = CallStatus.RanToCompletion,
                        Result = JToken.FromObject(result)
                    };
                }
                var uri = new Uri(request.RequesterUri, Url.Route("PostCallCompletion", null));
                var response = await client.PostAsJsonAsync(uri, completion, cancellationToken);
                var error = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            });
        }
        
        [Route("completions", Name = "PostCallCompletion")]
        public void PostCallCompletion(CallCompletion completion)
        {
            switch (completion.Status)
            {
                case CallStatus.RanToCompletion:
                    observer.OnResult(completion.CallId, completion.Result);
                    break;
                case CallStatus.Canceled:
                    observer.OnCancellation(completion.CallId);
                    break;
                case CallStatus.Faulted:
                    observer.OnException(completion.CallId, new Domain.Models.InvocationException(completion.Exception.Content));
                    break;
                default:
                    throw new Exception("This shouldn't ever happen.");
            }
        }
    }
}