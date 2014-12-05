using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "call_request")]
    public class CallRequest
    {
        [DataMember(Name = "requester_uri")]
        public Uri RequesterUri { get; set; }

        [DataMember(Name = "call_id", IsRequired = true)]
        public Guid CallId { get; set; }

        [DataMember(Name = "function", IsRequired = true)]
        public Function Function { get; set; }

        [DataMember(Name = "arguments", IsRequired = true)]
        public IList<JToken> Arguments { get; set; }
    }
}
