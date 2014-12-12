using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "void_argument")]
    public class VoidArgument : Argument
    {
        public override ArgumentKind Kind
        {
            get { return ArgumentKind.Void; }
        }
    }
}
