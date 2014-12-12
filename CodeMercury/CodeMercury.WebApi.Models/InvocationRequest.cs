using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "invocation_request")]
    public class InvocationRequest
    {
        [DataMember(Name = "requester_uri", IsRequired = true)]
        public Uri RequesterUri { get; set; }

        [DataMember(Name = "invocation_id", IsRequired = true)]
        public Guid InvocationId { get; set; }

        [DataMember(Name = "object", IsRequired = false)]
        public JToken Object { get; set; }

        [DataMember(Name = "method", IsRequired = true)]
        public Method Method { get; set; }

        [DataMember(Name = "arguments", IsRequired = true)]
        public IList<JToken> Arguments { get; set; }
    }
}
