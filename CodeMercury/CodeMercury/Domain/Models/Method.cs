using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class Method
    {
        public Type DeclaringType { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyCollection<Parameter> Parameters { get; private set; }

        public Method(Type declaringType, string name, IEnumerable<Parameter> parameters)
        {
            this.DeclaringType = declaringType;
            this.Name = name;
            this.Parameters = parameters.ToList().AsReadOnly();
        }

        public Method(MethodInfo methodInfo)
        {
            this.DeclaringType = methodInfo.DeclaringType;
            this.Name = methodInfo.Name;
            this.Parameters = methodInfo.GetParameters().Select(parameter => new Parameter(parameter)).ToList().AsReadOnly();
        }

        public MethodInfo ToMethodInfo()
        {
            return DeclaringType.GetMethod(Name, Parameters.Select(parameter => parameter.ParameterType).ToArray());
        }
    }
}
