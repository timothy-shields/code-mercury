using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "call_exception")]
    public class CallException
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}
