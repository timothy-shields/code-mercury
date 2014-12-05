using CodeMercury.WebApi.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Collections.Concurrent;
using CodeMercury.Components;
using System.Net.Http;

namespace CodeMercury.WebApi.Controllers
{
    public class CallController : ApiController
    {
        private readonly IInvoker invoker;
        private readonly HttpClient client;

        public CallController(IInvoker invoker)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException("invoker");
            }

            this.invoker = invoker;
            this.client = new HttpClient();
        }

        [Route("calls")]
        public CallReceipt PostCall(CallRequest request)
        {
            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                var method = ConvertFunction(request.Function);
                var arguments = Enumerable.Zip(request.Function.Parameters, request.Arguments, ConvertParameterAndArgument).ToArray();
                var result = await invoker.InvokeAsync(method, arguments);
                var uri = new Uri(request.RequesterUri, Url.Route("PutCallCompletion", new { call_id = request.CallId }));
                var completion = new CallCompletion();
                await client.PutAsJsonAsync(uri, completion, cancellationToken);
            });
            return new CallReceipt
            {
                CallId = request.CallId,
                CallUri = Url.Route("GetCall", new { call_id = request.CallId })
            };
        }

        private MethodInfo ConvertFunction(Function function)
        {
            return function.DeclaringType.GetMethod(function.Name, function.Parameters.Select(parameter => parameter.ParameterType).ToArray());
        }

        private object ConvertParameterAndArgument(Parameter parameter, JToken argument)
        {
            if (parameter.ParameterType == typeof(CancellationToken))
            {
                return cancellationTokenSource.Token;
            }
            return argument.ToObject(parameter.ParameterType);
        }

        [Route("calls/{call_id}", Name = "GetCall")]
        public Task<Call> GetCall(Guid call_id)
        {
            throw new NotImplementedException();
        }

        [Route("calls/{call_id}/completion", Name = "PutCallCompletion")]
        public Task PutCallCompletion(Guid call_id, CallCompletion completion)
        {
        }
    }
}