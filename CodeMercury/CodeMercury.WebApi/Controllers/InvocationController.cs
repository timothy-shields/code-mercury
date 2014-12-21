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
using CodeMercury.Services;

namespace CodeMercury.WebApi.Controllers
{
    public class InvocationController : ApiController
    {
        private readonly IInvoker invoker;
        private readonly IProxyContainer proxyContainer;

        private readonly HttpClient client;

        public InvocationController(IInvoker invoker, IProxyContainer proxyContainer)
        {
            this.invoker = invoker;
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
            if (result is TaskArgument)
            {
                return new WebApi.Models.TaskArgument
                {
                    Result = ConvertResult(method, result.CastTo<TaskArgument>().Result)
                };
            }
            if (result is ValueArgument)
            {
                return new WebApi.Models.ValueArgument
                {
                    Type = method.UnwrappedReturnType,
                    Value = JToken.FromObject(result.CastTo<ValueArgument>().Value)
                };   
            }
            if (result is VoidArgument)
            {
                return new WebApi.Models.VoidArgument();
            }
            throw new CodeMercuryBugException();
        }
    }
}