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
        [DataMember(Name = "result", IsRequired = true)]
        public Argument Result { get; set; }
    }
}
