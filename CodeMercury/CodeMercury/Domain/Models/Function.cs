using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    public class Function
    {
        public Type DeclaringType { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyCollection<Parameter> Parameters { get; private set; }

        public Function(Type declaringType, string name, IEnumerable<Parameter> parameters)
        {
            this.DeclaringType = declaringType;
            this.Name = name;
            this.Parameters = parameters.ToList().AsReadOnly();
        }
    }
}
