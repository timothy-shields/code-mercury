using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "proxy_argument")]
    public class ProxyArgument : Argument
    {
        public override ArgumentKind Kind
        {
            get { return ArgumentKind.Proxy; }
        }
        
        [DataMember(Name = "service_id")]
        public Guid ServiceId { get; set; }
    }
}
