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

        public bool ReturnsTask
        {
            get { return ReturnType.Equals(typeof(Task)) || ReturnType.IsSubclassOf(typeof(Task)); }
        }

        public Type UnwrappedReturnType
        {
            get
            {
                if (ReturnType.IsSubclassOf(typeof(Task)))
                {
                    return ReturnType.GetGenericArguments().Single();
                }
                if (ReturnType.Equals(typeof(Task)))
                {
                    return typeof(void);
                }
                return ReturnType;
            }
        }

        public Method(Type declaringType, string name, IEnumerable<Parameter> parameters)
        {
            this.DeclaringType = declaringType;
            this.Name = name;
            this.Parameters = parameters.ToList().AsReadOnly();

            this.lazyMethodInfo = new Lazy<MethodInfo>(GetMethodInfo);
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
    }
}
