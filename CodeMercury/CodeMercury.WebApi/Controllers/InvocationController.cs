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

        public InvocationController(IInvoker invoker)
        {
            this.invoker = invoker;
        }

        [Route("invocations")]
        public async Task<WebApi.Models.InvocationCompletion> PostInvocation(WebApi.Models.InvocationRequest invocationRequest, CancellationToken cancellationToken)
        {
            var @object = ConvertObject(invocationRequest.Object);
            var method = ConvertMethod(invocationRequest.Method);
            var arguments = ConvertArguments(invocationRequest);
            var invocation = new Invocation(@object, method, arguments);

            var result = await invoker.InvokeAsync(invocation);
            var completion = new WebApi.Models.InvocationCompletion
            {
                Result = ConvertResult(method.ReturnType, result)
            };
            return completion;
        }

        private Argument ConvertObject(WebApi.Models.Argument @object)
        {
            if (@object is WebApi.Models.ServiceArgument)
            {
                return new ServiceArgument(@object.CastTo<WebApi.Models.ServiceArgument>().ServiceId);
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
                method.Parameters.Select(parameter => new Parameter(parameter.ParameterType)).ToList().AsReadOnly());
        }

        private IReadOnlyCollection<Argument> ConvertArguments(WebApi.Models.InvocationRequest invocationRequest)
        {
            return Enumerable.Zip<WebApi.Models.Parameter, WebApi.Models.Argument, Argument>(
                invocationRequest.Method.Parameters,
                invocationRequest.Arguments,
                (parameter, argument) =>
                {
                    if (argument is WebApi.Models.ProxyArgument)
                    {
                        return new ProxyArgument(argument.CastTo<WebApi.Models.ProxyArgument>().ServiceId);
                    }
                    if (argument is WebApi.Models.ValueArgument)
                    {
                        return new ValueArgument(argument.CastTo<WebApi.Models.ValueArgument>().Value.ToObject(parameter.ParameterType));
                    }
                    throw new CodeMercuryBugException();
                })
                .ToList().AsReadOnly();
        }

        private WebApi.Models.Argument ConvertResult(Type type, Argument result)
        {
            if (result is CanceledArgument)
            {
                return new WebApi.Models.CanceledArgument();
            }
            if (result is ExceptionArgument)
            {
                return new WebApi.Models.ExceptionArgument
                {
                    Content = result.CastTo<ExceptionArgument>().Exception.ToString()
                };
            }
            if (result is TaskArgument)
            {
                Type resultType;
                if (type.IsSubclassOf(typeof(Task)))
                {
                    resultType = type.GetGenericArguments().Single();
                }
                else if (type.Equals(typeof(Task)))
                {
                    resultType = typeof(void);
                }
                else
                {
                    throw new CodeMercuryBugException();
                }
                return new WebApi.Models.TaskArgument
                {
                    Result = ConvertResult(resultType, result.CastTo<TaskArgument>().Result)
                };
            }
            if (result is ValueArgument)
            {
                return new WebApi.Models.ValueArgument
                {
                    Type = type,
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