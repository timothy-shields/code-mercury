using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "argument")]
    [JsonConverter(typeof(ArgumentJsonConverter))]
    public abstract class Argument
    {
        [DataMember(Name = "kind", IsRequired = true)]
        public abstract ArgumentKind Kind { get; }
    }
}
