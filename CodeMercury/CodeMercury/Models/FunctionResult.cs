using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Reactive;
using CodeMercury.Json;

namespace CodeMercury.Models
{
    public class FunctionResult
    {
        public Type Type { get; set; }
        public JToken Value { get; set; }

        public bool IsException { get { return Type.IsSubclassOf(typeof(Exception)); } }

        public static FunctionResult FromValue(object value, Type type)
        {
            return new FunctionResult
            {
                Type = type,
                Value = JsonHelper.ToJson(value)
            };
        }

        public static FunctionResult FromValue<T>(T value)
        {
            return new FunctionResult
            {
                Type = typeof(T),
                Value = JsonHelper.ToJson(value)
            };
        }

        public static FunctionResult FromException(Exception exception)
        {
            return new FunctionResult
            {
                Type = exception.GetType(),
                Value = JsonHelper.ToJson(exception.ToString())
            };
        }

        public object ToObject()
        {
            if (IsException)
                throw new FunctionException(Value.ToObject<string>());
            return JsonHelper.ToObject(Value, Type);
        }

        public override string ToString()
        {
            if (IsException)
                return Value.ToObject<string>();
            if (Type == typeof(void))
                return Type.Name;
            var _object = ToObject();
            return Type.Name + ": " + (_object != null ? _object.ToString() : "null");
        }
    }
}
