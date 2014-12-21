using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    [DataContract(Name = "task_argument")]
    public class TaskArgument : Argument
    {
        public override ArgumentKind Kind
        {
            get { return ArgumentKind.Task; }
        }
        
        [DataMember(Name = "result")]
        public Argument Result { get; set; }
    }
}
