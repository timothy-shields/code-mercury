using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "parameter")]
    public class Parameter
    {
        [DataMember(Name = "parameter_type")]
        public Type ParameterType { get; set; }
    }
}
