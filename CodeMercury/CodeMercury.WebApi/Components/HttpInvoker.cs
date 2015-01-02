using CodeMercury.Components;
using CodeMercury.Domain.Models;
using CodeMercury.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Components
{
    public class HttpInvoker : IInvoker
    {
        private readonly Uri requesterUri;
        private readonly HttpClient client;
        private readonly IServiceContainer serviceContainer;

        public HttpInvoker(Uri requesterUri, Uri serverUri, IServiceContainer serviceContainer)
        {
            this.requesterUri = requesterUri;
            this.client = new HttpClient { BaseAddress = serverUri };
            this.serviceContainer = serviceContainer;
        }

        public async Task<Argument> InvokeAsync(Invocation invocation)
        {
            using (var scopedServiceContainer = new ScopedServiceContainer(serviceContainer))
            {
                var request = new WebApi.Models.InvocationRequest
                {
                    RequesterUri = requesterUri,
                    Object = ConvertObject(invocation.Object),
                    Method = ConvertMethod(invocation.Method),
                    Arguments = Enumerable.Zip(invocation.Method.Parameters, invocation.Arguments,
                        (parameter, argument) => ConvertArgument(parameter, argument)).ToList()
                };
                var response = await client.PostAsJsonAsync("invocations", request);
                response.EnsureSuccessStatusCode();
                var completion = await response.Content.ReadAsAsync<WebApi.Models.InvocationCompletion>();
                var result = ConvertResult(completion.Result);
                return result;
            }
        }

        private static Argument ConvertResult(WebApi.Models.Argument argument)
        {
            if (argument is WebApi.Models.CanceledArgument)
            {
                return new CanceledArgument();
            }
            if (argument is WebApi.Models.ExceptionArgument)
            {
                return new ExceptionArgument(new InvocationException(argument.CastTo<WebApi.Models.ExceptionArgument>().Content));
            }
            if (argument is WebApi.Models.TaskArgument)
            {
                return new TaskArgument(ConvertResult(argument.CastTo<WebApi.Models.TaskArgument>().Result));
            }
            if (argument is WebApi.Models.ValueArgument)
            {
                var valueArgument = argument.CastTo<WebApi.Models.ValueArgument>();
                return new ValueArgument(valueArgument.Value.ToObject(valueArgument.Type));
            }
            if (argument is WebApi.Models.VoidArgument)
            {
                return new VoidArgument();
            }
            throw new CodeMercuryBugException();
        }

        private static WebApi.Models.Argument ConvertObject(Argument @object)
        {
            if (@object is ServiceArgument)
            {
                return new WebApi.Models.ServiceArgument
                {
                    ServiceId = @object.CastTo<ServiceArgument>().ServiceId
                };
            }
            if (@object is StaticArgument)
            {
                return new WebApi.Models.StaticArgument();
            }
            if (@object is ValueArgument)
            {
                return new WebApi.Models.ValueArgument
                {
                    Value = JToken.FromObject(@object.CastTo<ValueArgument>().Value)
                };
            }
            throw new CodeMercuryBugException();
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
                        ParameterType = parameter.ParameterType
                    })
                    .ToList()
            };
        }

        private WebApi.Models.Argument ConvertArgument(Parameter parameter, Argument argument)
        {
            if (argument is ValueArgument)
            {
                var value = argument.CastTo<ValueArgument>().Value;
                if (IsProxyable(parameter.ParameterType))
                {
                    var serviceId = Guid.NewGuid();
                    serviceContainer.Register(serviceId, value);
                    return new WebApi.Models.ProxyArgument
                    {
                        ServiceId = serviceId
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
            throw new CodeMercuryBugException();
        }

        private static bool IsProxyable(Type parameterType)
        {
            return parameterType.IsInterface;
        }
    }
}
