using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Domain.Models
{
    /// <summary>
    /// Represents a method.
    /// </summary>
    public class Method
    {
        /// <summary>
        /// The type that declares the method.
        /// </summary>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// The name of the method.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The parameters of the method.
        /// </summary>
        public IReadOnlyCollection<Type> ParameterTypes { get; private set; }

        private readonly Lazy<MethodInfo> lazyMethodInfo;

        /// <summary>
        /// The MethodInfo for this method.
        /// </summary>
        public MethodInfo MethodInfo
        {
            get { return lazyMethodInfo.Value; }
        }

        /// <summary>
        /// The return type of this method.
        /// </summary>
        public Type ReturnType
        {
            get { return MethodInfo.ReturnType; }
        }

        public Method(Type declaringType, string name, IReadOnlyCollection<Type> parameterTypes)
        {
            this.DeclaringType = declaringType;
            this.Name = name;
            this.ParameterTypes = parameterTypes;

            this.lazyMethodInfo = new Lazy<MethodInfo>(GetMethodInfo);
        }

        /// <summary>
        /// Get another method that differs from this one in only its declaring type.
        /// </summary>
        /// <param name="declaringType">The type declaring the other method.</param>
        /// <returns>The other method.</returns>
        public Method WithDeclaringType(Type declaringType)
        {
            return new Method(declaringType, Name, ParameterTypes);
        }

        private MethodInfo GetMethodInfo()
        {
            var bindingFlags =
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance;
            var methodInfo = DeclaringType.GetMethod(Name, bindingFlags, null, ParameterTypes.ToArray(), null);
            return methodInfo;
        }

        public override string ToString()
        {
            return string.Format("Method({0}.{1}({2}))", DeclaringType.Name, Name, string.Join(", ", ParameterTypes));
        }
    }
}
