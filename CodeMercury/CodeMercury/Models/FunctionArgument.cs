using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq.Expressions;
using CodeMercury.Expressions;
using CodeMercury.Json;

namespace CodeMercury.Models
{
    /// <summary>
    /// Represents an argument in a FunctionApplication, F(x1, x2, ..., xN).
    /// </summary>
    public class FunctionArgument
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public JToken Value { get; set; }

        /// <summary>
        /// Create a FunctionArgument from ParameterInfo and an Expression to evaluate.
        /// </summary>
        public static FunctionArgument FromParameterAndArgument(ParameterInfo parameterInfo, Expression argumentExpression)
        {
            return new FunctionArgument
            {
                Type = parameterInfo.ParameterType,
                Name = parameterInfo.Name,
                Value = JsonHelper.ToJson(ExpressionHelper.Evaluate(argumentExpression))
            };
        }

        public object ToObject()
        {
            return JsonHelper.ToObject(Value, Type);
        }

        public override string ToString()
        {
            var _object = ToObject();
            return Type.Name + " " + Name + " = " + (_object != null ? _object.ToString() : "null");
        }
    }
}
