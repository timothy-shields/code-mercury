using CodeMercury.Domain.Models;
using CodeMercury.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Components
{
    public class ArgumentMaterializer : IArgumentMaterializer
    {
        public object Materialize(Type type, Argument argument)
        {
            if (argument is CanceledArgument)
            {
                throw new OperationCanceledException();
            }
            if (argument is ExceptionArgument)
            {
                var exceptionArgument = argument.CastTo<ExceptionArgument>();
                var capturedException = CapturedException.Create(exceptionArgument.Exception);
                throw capturedException;
            }
            if (argument is TaskArgument)
            {
                var taskArgument = (TaskArgument)argument;
                var resultType = type.IsGenericType ? type.GetGenericArguments().Single() : typeof(object);
                if (taskArgument.Result is CanceledArgument)
                {
                    return CanceledTask(resultType);
                }
                if (taskArgument.Result is ExceptionArgument)
                {
                    var exceptionArgument = taskArgument.Result.CastTo<ExceptionArgument>();
                    var capturedException = CapturedException.Create(exceptionArgument.Exception);
                    return FaultedTask(resultType, capturedException);
                }
                return CompletedTask(resultType, Materialize(resultType, taskArgument.Result));
            }
            if (argument is ValueArgument)
            {
                return argument.CastTo<ValueArgument>().Value;
            }
            if (argument is VoidArgument)
            {
                return null;
            }
            throw new CodeMercuryBugException();
        }

        private static dynamic CreateTaskCompletionSource(Type resultType)
        {
            return Activator.CreateInstance(typeof(TaskCompletionSource<>).MakeGenericType(resultType));
        }

        private static object CompletedTask(Type resultType, dynamic result)
        {
            var tcs = CreateTaskCompletionSource(resultType);
            tcs.SetResult(result);
            return tcs.Task;
        }

        private static object CanceledTask(Type resultType)
        {
            var tcs = CreateTaskCompletionSource(resultType);
            tcs.SetCanceled();
            return tcs.Task;
        }

        private static object FaultedTask(Type resultType, Exception exception)
        {
            var tcs = CreateTaskCompletionSource(resultType);
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}
