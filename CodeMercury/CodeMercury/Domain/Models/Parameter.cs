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

        public Parameter(Type parameterType)
        {
            this.ParameterType = parameterType;
        }

        public override string ToString()
        {
            return string.Format("Parameter({0})", ParameterType.Name);
        }
    }
}
