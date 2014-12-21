using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "argument_kind")]
    public enum ArgumentKind
    {
        [EnumMember(Value = "proxy")]
        Proxy,

        [EnumMember(Value = "service")]
        Service,

        [EnumMember(Value = "static")]
        Static,

        [EnumMember(Value = "task")]
        Task,

        [EnumMember(Value = "value")]
        Value,

        [EnumMember(Value = "void")]
        Void
    }
}
