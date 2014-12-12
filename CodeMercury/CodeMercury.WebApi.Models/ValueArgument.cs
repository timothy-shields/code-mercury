using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "value_argument")]
    public class ValueArgument : Argument
    {
        public override ArgumentKind Kind
        {
            get { return ArgumentKind.Value; }
        }

        [DataMember(Name = "type")]
        public Type Type { get; set; }

        [DataMember(Name = "value")]
        public JToken Value { get; set; }
    }
}
