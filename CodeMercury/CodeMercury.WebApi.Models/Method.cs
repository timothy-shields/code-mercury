using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "method")]
    public class Method
    {
        [DataMember(Name = "declaring_type")]
        public Type DeclaringType { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "parameters")]
        public IList<Parameter> Parameters { get; set; }
    }
}
