using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class Parameter
    {
        public Type ParameterType { get; private set; }
        public string Name { get; private set; }

        public Parameter(Type parameterType, string name)
        {
            this.ParameterType = parameterType;
            this.Name = name;
        }
    }
}
