using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents a method parameter.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// The parameter type.
        /// </summary>
        public Type ParameterType { get; private set; }
        
        /// <summary>
        /// The parameter name.
        /// </summary>
        public string Name { get; private set; }

        public Parameter(Type parameterType, string name)
        {
            this.ParameterType = parameterType;
            this.Name = name;
        }

        public Parameter(ParameterInfo parameterInfo)
        {
            this.ParameterType = parameterInfo.ParameterType;
            this.Name = parameterInfo.Name;
        }

        public override string ToString()
        {
            return string.Format("Parameter({0} {1})", ParameterType.Name, Name);
        }
    }
}
