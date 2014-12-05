using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "call")]
    public class Call
    {
        [DataMember(Name = "call_id")]
        public Guid CallId { get; set; }

        [DataMember(Name = "function", IsRequired = true)]
        public Function Function { get; set; }

        [DataMember(Name = "arguments", IsRequired = true)]
        public IList<JToken> Arguments { get; set; }

        [DataMember(Name = "status")]
        public CallStatus Status { get; set; }

        [DataMember(Name = "result")]
        public JToken Result { get; set; }

        [DataMember(Name = "exception")]
        public CallException Exception { get; set; }
    }
}
