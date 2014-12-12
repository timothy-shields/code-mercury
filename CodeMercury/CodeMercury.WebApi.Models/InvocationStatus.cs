using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "invocation_status")]
    public enum InvocationStatus
    {
        [EnumMember(Value = "ran_to_completion")]
        RanToCompletion,

        [EnumMember(Value = "canceled")]
        Canceled,

        [EnumMember(Value = "faulted")]
        Faulted,
    }
}
