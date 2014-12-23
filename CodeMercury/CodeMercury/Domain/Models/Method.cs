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

        private readonly Lazy<MethodInfo> lazyMethodInfo;

        public MethodInfo MethodInfo
        {
            get { return lazyMethodInfo.Value; }
        }

        public Type ReturnType
        {
            get { return MethodInfo.ReturnType; }
        }

        public Method(Type declaringType, string name, IReadOnlyCollection<Parameter> parameters)
        {
            this.DeclaringType = declaringType;
            this.Name = name;
            this.Parameters = parameters;

            this.lazyMethodInfo = new Lazy<MethodInfo>(GetMethodInfo);
        }

        public Method WithDeclaringType(Type declaringType)
        {
            return new Method(declaringType, Name, Parameters);
        }

        private MethodInfo GetMethodInfo()
        {
            var bindingFlags =
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance;
            var methodInfo = DeclaringType.GetMethod(Name, bindingFlags, null, Parameters.Select(parameter => parameter.ParameterType).ToArray(), null);
            return methodInfo;
        }

        public override string ToString()
        {
            return string.Format("Method({0}.{1}({2}))", DeclaringType.Name, Name, string.Join(", ", Parameters));
        }
    }
}
