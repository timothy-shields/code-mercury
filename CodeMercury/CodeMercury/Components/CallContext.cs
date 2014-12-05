using CodeMercury.WebApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class CallContext : ICallContext
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private MethodInfo method;
        private object[] arguments;

        public Type ReturnType
        {
            get { return method.ReturnType; }
        }

        public CallContext(Call call)
        {
            method = ConvertFunction(call.Function);
            arguments = Enumerable.Zip(call.Function.Parameters, call.Arguments, ConvertParameterAndArgument).ToArray();
        }

        public object Invoke()
        {
            var returnType = method.ReturnType;
            if (returnType.IsSubclassOf(typeof(Task)))
            {
                var resultType = method.GetGenericArguments().First();
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {

                });
            }
            else if (returnType.Equals(typeof(Task)))
            {
            }
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
    }
}
