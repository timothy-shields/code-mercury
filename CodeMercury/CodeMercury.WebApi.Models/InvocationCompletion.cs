using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "invocation_completion")]
    public class InvocationCompletion
    {
        [DataMember(Name = "invocation_id", IsRequired = true)]
        public Guid InvocationId { get; set; }

        [DataMember(Name = "status", IsRequired = true)]
        public InvocationStatus Status { get; set; }

        [DataMember(Name = "result")]
        public Argument Result { get; set; }

        [DataMember(Name = "exception")]
        public InvocationException Exception { get; set; }
    }
}
