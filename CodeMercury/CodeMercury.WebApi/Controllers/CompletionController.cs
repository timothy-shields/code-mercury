using CodeMercury.Domain.Models;
using CodeMercury.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CodeMercury.WebApi.Controllers
{
    public class CompletionController : ApiController
    {
        private readonly IInvocationObserver invocationObserver;

        public CompletionController(IInvocationObserver invocationObserver)
        {
            this.invocationObserver = invocationObserver;
        }

        [Route("completions", Name = "PostInvocationCompletion")]
        public void PostInvocationCompletion(WebApi.Models.InvocationCompletion completion)
        {
            if (completion.Status == WebApi.Models.InvocationStatus.RanToCompletion)
            {
                invocationObserver.OnResult(completion.InvocationId, ConvertResult(completion.Result));
                return;
            }
            if (completion.Status == WebApi.Models.InvocationStatus.Canceled)
            {
                invocationObserver.OnCancellation(completion.InvocationId);
                return;
            }
            if (completion.Status == WebApi.Models.InvocationStatus.Faulted)
            {
                invocationObserver.OnException(completion.InvocationId, new InvocationException(completion.Exception.Content));
                return;
            }
            throw new CodeMercuryBugException();
        }

        private Argument ConvertResult(WebApi.Models.Argument argument)
        {
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
    }
}
