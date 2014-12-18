using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "service_argument")]
    public class ServiceArgument : Argument
    {
        public override ArgumentKind Kind
        {
            get { return ArgumentKind.Service; }
        }

        [DataMember(Name = "service_id")]
        public Guid ServiceId { get; set; }
    }
}
