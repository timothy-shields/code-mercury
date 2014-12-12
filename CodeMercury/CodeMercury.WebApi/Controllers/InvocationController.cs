using CodeMercury.Domain.Models;
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
    public class InvocationController : ApiController
    {
        private readonly IInvoker invoker;
        private readonly IInvocationObserver observer;
        private readonly IProxyContainer proxyContainer;

        private readonly HttpClient client;

        public InvocationController(IInvoker invoker, IInvocationObserver observer, IProxyContainer proxyContainer)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException("invoker");
            }
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }
            if (proxyContainer == null)
            {
                throw new ArgumentNullException("proxyContainer");
            }

            this.invoker = invoker;
            this.observer = observer;
            this.proxyContainer = proxyContainer;

            this.client = new HttpClient();
        }

        [Route("invocations")]
        public void PostInvocation(WebApi.Models.InvocationRequest invocationRequest)
        {
            BackgroundTaskManager.Run(async () =>
            {
                var cancellationToken = BackgroundTaskManager.Shutdown;

                Argument @object = null; //TODO Handle instance method calls
                var method = new Method(
                    invocationRequest.Method.DeclaringType,
                    invocationRequest.Method.Name,
                    invocationRequest.Method.Parameters.Select(parameter => new Parameter(parameter.ParameterType, parameter.Name)));
                var arguments = Enumerable.Zip<WebApi.Models.Parameter, JToken, Argument>(
                    invocationRequest.Method.Parameters,
                    invocationRequest.Arguments,
                    (parameter, argument) =>
                    {
                        var argumentKind = argument["kind"].ToObject<WebApi.Models.ArgumentKind>();
                        if (argumentKind == WebApi.Models.ArgumentKind.Proxy)
                        {
                            var proxyArgument = argument.ToObject<WebApi.Models.ProxyArgument>();
                            proxyContainer.Register(proxyArgument.ProxyId, parameter.ParameterType);
                            return new ProxyArgument(proxyArgument.ProxyId);
                        }
                        else if (argumentKind == WebApi.Models.ArgumentKind.Value)
                        {
                            var valueArgument = argument.ToObject<WebApi.Models.ValueArgument>();
                            return new ValueArgument(valueArgument.Value.ToObject(parameter.ParameterType));
                        }
                        else
                        {
                            throw new CodeMercuryBugException();
                        }
                    });
                var invocation = new Invocation(@object, method, arguments);
                
                WebApi.Models.InvocationCompletion completion = null;
                Argument resultArgument = null;
                try
                {
                    resultArgument = await invoker.InvokeAsync(invocation);
                }
                catch (OperationCanceledException)
                {
                    completion = new WebApi.Models.InvocationCompletion
                    {
                        InvocationId = invocationRequest.InvocationId,
                        Status = WebApi.Models.InvocationStatus.Canceled
                    };
                }
                catch (Exception exception)
                {
                    completion = new WebApi.Models.InvocationCompletion
                    {
                        InvocationId = invocationRequest.InvocationId,
                        Status = WebApi.Models.InvocationStatus.Faulted,
                        Exception = new WebApi.Models.InvocationException
                        {
                            Content = exception.ToString()
                        }
                    };
                }
                if (completion == null)
                {
                    completion = new WebApi.Models.InvocationCompletion
                    {
                        InvocationId = invocationRequest.InvocationId,
                        Status = WebApi.Models.InvocationStatus.RanToCompletion,
                        Result = JToken.FromObject(resultArgument) //TODO handle ProxyArgument
                    };
                }
                var uri = new Uri(invocationRequest.RequesterUri, Url.Route("PostInvocationCompletion", null));
                var response = await client.PostAsJsonAsync(uri, completion, cancellationToken);
                var error = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            });
        }
        
        [Route("completions", Name = "PostInvocationCompletion")]
        public void PostInvocationCompletion(WebApi.Models.InvocationCompletion completion)
        {
            if (completion.Status == WebApi.Models.InvocationStatus.RanToCompletion)
            {
                var resultArgumentKind = completion.Result["kind"].ToObject<WebApi.Models.ArgumentKind>();
                if (resultArgumentKind == WebApi.Models.ArgumentKind.Value)
                {
                    var valueArgument = completion.Result.ToObject<WebApi.Models.ValueArgument>();
                    observer.OnResult(completion.InvocationId, new ValueArgument(valueArgument.Value.ToObject(valueArgument.Type)));
                }
                else if (resultArgumentKind == WebApi.Models.ArgumentKind.Void)
                {
                    observer.OnResult(completion.InvocationId, new VoidArgument());
                }
                else
                {
                    throw new CodeMercuryBugException();
                }
            }
            else if (completion.Status == WebApi.Models.InvocationStatus.Canceled)
            {
                observer.OnCancellation(completion.InvocationId);
            }
            else if (completion.Status == WebApi.Models.InvocationStatus.Faulted)
            {
                observer.OnException(completion.InvocationId, new InvocationException(completion.Exception.Content));
            }
            else
            {
                throw new CodeMercuryBugException();
            }
        }
    }
}