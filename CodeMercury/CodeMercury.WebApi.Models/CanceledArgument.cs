using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "canceled_argument")]
    public class CanceledArgument : Argument
    {
        public override ArgumentKind Kind
        {
            get { return ArgumentKind.Canceled; }
        }
    }
}
