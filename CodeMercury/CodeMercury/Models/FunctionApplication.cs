using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeMercury.Models
{
    /// <summary>
    /// Represents an instance of a function application, F(x1, x2, ..., xN), yet to be executed.
    /// </summary>
    public class FunctionApplication
    {
        /// <summary>
        /// The type that declares the function being called.
        /// </summary>
        public Type DeclaringType { get; set; }

        /// <summary>
        /// The name of the function being called.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The arguments being passed in as the function's parameters.
        /// </summary>
        public List<FunctionArgument> Arguments { get; set; }

        /// <summary>
        /// Create a FunctionApplication from a LambdaExpression whose body is a MethodCallExpression to call an instance function.
        /// </summary>
        public static FunctionApplication FromExpression<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return FromExpression(expression.Parameters.First(), expression.Body as MethodCallExpression);
        }

        /// <summary>
        /// Create a FunctionApplication from a LambdaExpression whose body is a MethodCallExpression to call a static function.
        /// </summary>
        public static FunctionApplication FromExpression<TResult>(Expression<Func<TResult>> expression)
        {
            return FromExpression(null, expression.Body as MethodCallExpression);
        }

        /// <summary>
        /// Create a FunctionApplication from a LambdaExpression whose body is a MethodCallExpression to call an instance function.
        /// </summary>
        //public static FunctionApplication FromExpression<T>(Expression<Action<T>> expression)
        //{
        //    return FromExpression(expression.Parameters.First(), expression.Body as MethodCallExpression);
        //}

        /// <summary>
        /// Create a FunctionApplication from a LambdaExpression whose body is a MethodCallExpression to call a static function.
        /// </summary>
        //public static FunctionApplication FromExpression(Expression<Action> expression)
        //{
        //    return FromExpression(null, expression.Body as MethodCallExpression);
        //}

        private static FunctionApplication FromExpression(ParameterExpression parameter, MethodCallExpression expression)
        {
            if (expression.Object != parameter)
                throw new InvalidOperationException();

            return new FunctionApplication
            {
                DeclaringType = expression.Method.DeclaringType,
                Name = expression.Method.Name,
                Arguments =
                    Enumerable.Zip(
                        expression.Method.GetParameters(),
                        expression.Arguments,
                        FunctionArgument.FromParameterAndArgument)
                    .ToList()
            };
        }

        /// <summary>
        /// Invoke the function to obtain the return value y = F(x1, x2, ..., xN) or any exception thrown while executing it.
        /// The Identity of the FunctionResult returned will be the Identity of this FunctionApplication.
        /// </summary>
        public FunctionResult Invoke(object obj)
        {
            var argumentTypes = Arguments.Select(argument => argument.Type).ToArray();
            var argumentValues = Arguments.Select(argument => argument.ToObject()).ToArray();
            var method = DeclaringType.GetMethod(Name, argumentTypes);

            if (method.IsStatic != (obj == null))
                throw new InvalidOperationException();

            try
            {
                return FunctionResult.FromValue(method.Invoke(obj, argumentValues), method.ReturnType);
            }
            catch (Exception e)
            {
                return FunctionResult.FromException(e);
            }
        }

        public override string ToString()
        {
            return DeclaringType.Name + "." + Name + "(" + string.Join(", ", Arguments) + ")";
        }
    }
}
