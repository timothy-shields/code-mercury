using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "exception_argument")]
    public class ExceptionArgument : Argument
    {
        public override ArgumentKind Kind
        {
            get { return ArgumentKind.Exception; }
        }

        [DataMember(Name = "type")]
        public Type Type { get; set; }

        [DataMember(Name = "contents")]
        public List<string> Contents { get; set; }        
    }
}
