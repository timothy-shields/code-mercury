using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "call_receipt")]
    public class CallReceipt
    {
        [DataMember(Name = "call_id")]
        public Guid CallId { get; set; }

        [DataMember(Name = "call_uri")]
        public string CallUri { get; set; }
    }
}
