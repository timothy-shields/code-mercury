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

                var @object = ConvertObject(invocationRequest.Object);
                var method = ConvertMethod(invocationRequest.Method);
                var arguments = ConvertArguments(invocationRequest);
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
                        Result = ConvertResult(method, resultArgument)
                    };
                }
                var uri = new Uri(invocationRequest.RequesterUri, Url.Route("PostInvocationCompletion", null));
                var response = await client.PostAsJsonAsync(uri, completion, cancellationToken);
                var error = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            });
        }

        private Argument ConvertObject(WebApi.Models.Argument @object)
        {
            if (@object is WebApi.Models.ServiceArgument)
            {
                return new ServiceArgument(@object.CastTo<ServiceArgument>().ServiceId);
            }
            if (@object is WebApi.Models.StaticArgument)
            {
                return new StaticArgument();
            }
            throw new CodeMercuryBugException();
        }

        private static Method ConvertMethod(WebApi.Models.Method method)
        {
            return new Method(
                method.DeclaringType,
                method.Name,
                method.Parameters.Select(parameter => new Parameter(parameter.ParameterType, parameter.Name)));
        }

        private IEnumerable<Argument> ConvertArguments(WebApi.Models.InvocationRequest invocationRequest)
        {
            return Enumerable.Zip<WebApi.Models.Parameter, WebApi.Models.Argument, Argument>(
                invocationRequest.Method.Parameters,
                invocationRequest.Arguments,
                (parameter, argument) =>
                {
                    if (argument is WebApi.Models.ProxyArgument)
                    {
                        var serviceId = argument.CastTo<WebApi.Models.ProxyArgument>().ServiceId;
                        proxyContainer.Register(serviceId, parameter.ParameterType);
                        return new ProxyArgument(serviceId);
                    }
                    if (argument is WebApi.Models.ValueArgument)
                    {
                        return new ValueArgument(argument.CastTo<WebApi.Models.ValueArgument>().Value.ToObject(parameter.ParameterType));
                    }
                    throw new CodeMercuryBugException();
                });
        }

        private WebApi.Models.Argument ConvertResult(Method method, Argument result)
        {
            if (result is ValueArgument)
            {
                var resultType = method.ToMethodInfo().ReturnType;
                if (resultType.IsSubclassOf(typeof(Task)))
                {
                    resultType = resultType.GetGenericArguments().Single();
                }
                return new WebApi.Models.ValueArgument
                {
                    Type = resultType,
                    Value = JToken.FromObject(result.CastTo<ValueArgument>().Value)
                };   
            }
            if (result is VoidArgument)
            {
                return new WebApi.Models.VoidArgument();
            }
            throw new CodeMercuryBugException();
        }
        
        [Route("completions", Name = "PostInvocationCompletion")]
        public void PostInvocationCompletion(WebApi.Models.InvocationCompletion completion)
        {
            if (completion.Status == WebApi.Models.InvocationStatus.RanToCompletion)
            {
                var result = completion.Result;
                if (result is WebApi.Models.ValueArgument)
                {
                    var valueArgument = (WebApi.Models.ValueArgument)result;
                    observer.OnResult(completion.InvocationId, new ValueArgument(valueArgument.Value.ToObject(valueArgument.Type)));
                }
                else if (result is WebApi.Models.VoidArgument)
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